using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;
using Android.Support.V7.Widget;
using Cnblogs.ApiModel;
using Android.Support.V4.Widget;
using Android.Graphics;
using System.Threading.Tasks;
using Cnblogs.HttpClient;
using Cnblogs.XamarinAndroid.UI.Widgets;
namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/myBlog",Theme ="@style/AppTheme")]
    public  class MyBlogActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
    {
        
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.myBlog;
            }
        }

        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<Article> adapter;
        private LinearLayout ly_expire;
        private TextView tv_startLogin;
        public string   blogApp; //������
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private List<Article> articleList = new List<Article>();
        private TextView tv_userName;
        private TextView tv_articleCount;
        private ImageView headPic;
        internal static void Enter(string  _blogApp,Context context)
        {
            Intent intent = new Intent(context, typeof(MyBlogActivity));
            intent.PutExtra("blogApp", _blogApp);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            blogApp = Intent.GetStringExtra("blogApp");
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.myBlog));
            tv_articleCount = FindViewById<TextView>(Resource.Id.tv_articleCount);
            tv_userName = FindViewById<TextView>(Resource.Id.tv_userName);
            headPic = FindViewById<ImageView>(Resource.Id.headPic);
            UserBlog userBlog = UserBlogShared.GetUserBlog(this);
            UserInfo userInfo = UserInfoShared.GetUserInfo(this);
            tv_articleCount.Text = "�������� "+userBlog.PostCount;
            tv_userName.Text = userInfo.DisplayName;
            //��ʾͼƬ����
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .CacheOnDisk(true)
                 // .Displayer(new DisplayerImageCircle(20))
                  .Build();
            ImageLoader.Instance.DisplayImage(userInfo.Avatar, headPic, options);
            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);


            ly_expire = FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = FindViewById<TextView>(Resource.Id.tv_startLogin);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this));

            Token token = UserTokenUtil.GetToken(this);
            if (token.IsExpire)
            {
                ly_expire.Visibility = ViewStates.Visible;
                _swipeRefreshLayout.Visibility = ViewStates.Gone;
                tv_startLogin.Click += (s, e) =>
                {
                    StartActivity(new Intent(this, typeof(loginactivity)));
                };
                return;
            }
            else
            {
                ly_expire.Visibility = ViewStates.Gone;
                _swipeRefreshLayout.Visibility = ViewStates.Visible;
                articleList = await SQLiteUtil.SelectArticleList(Constact.PageSize);
                if (articleList != null)
                {
                    initRecycler();
                }
                OnRefresh();
            }
        }

        private async void LoadMore()
        {
            pageIndex++;
            var result = await UserInfoService.GetMyBlogPosts(UserTokenUtil.GetToken(this), blogApp, pageIndex);
            if (result.Success)
            {
                var tempList = result.Data;
                articleList.AddRange(tempList);
                adapter.SetNewData(articleList);
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<Article>(this, articleList, Resource.Layout.item_recyclerview_myblog, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                    System.Diagnostics.Debug.Write(position, tag);
                    var intent = new Intent(this, typeof(DetailBlogActivity));
                    intent.PutExtra("id", int.Parse(tag));
                    StartActivity(intent);
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                //AlertUtil.ToastShort(this, tag);
            };
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                holder.SetText(Resource.Id.tv_postDate, articleList[position].PostDate.ToString("yyyy-MM-dd HH:ss"));
                holder.SetText(Resource.Id.tv_viewCount, articleList[position].ViewCount + " " + read);
                holder.SetText(Resource.Id.tv_commentCount, articleList[position].CommentCount + " " + comment);
                holder.SetText(Resource.Id.tv_description, articleList[position].Description);
                holder.SetText(Resource.Id.tv_diggCount, articleList[position].Diggcount + " " + digg);
                holder.SetText(Resource.Id.tv_title, articleList[position].Title.Replace("\n","").Replace("  ",""));
                holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = articleList[position].Id.ToString();
                holder.SetTagUrl(Resource.Id.iv_avatar, articleList[position].Avatar);
                //ImageLoader.Instance.DisplayImage(articleList[position].Avatar, Resource.Id.iv_avatar);
            };
        }
    

        public async void OnRefresh()
        {
            _swipeRefreshLayout.Post(()=> {
                _swipeRefreshLayout.Refreshing = true;
            });
            pageIndex = 1;
            var result =  await UserInfoService.GetMyBlogPosts(UserTokenUtil.GetToken(this), blogApp, pageIndex);
            if (result.Success)
            {
                articleList = result.Data;
                initRecycler();
                if (articleList.Count != 0)
                {
                    await SQLiteUtil.UpdateArticleList(result.Data);
                }
                _swipeRefreshLayout.Post(() => {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
                _swipeRefreshLayout.Post(() => {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
        }
    }
}
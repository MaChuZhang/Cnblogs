using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using Cnblogs.XamarinAndroid;
using Cnblogs.HttpClient;
using Cnblogs.ApiModel;
using Cnblogs.XamarinAndroid.UI;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;
using System.Threading.Tasks;
using System.Threading;
using Cnblogs.XamarinAndroid.UI.Widgets;

namespace Cnblogs.XamarinAndroid
{
    public class RecyclerViewFragment : Fragment,SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<Article> adapter;
        public int position;
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private List<Article> articleList = new List<Article>();
        private DateTime lastRefreshTime ;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            
            // Create your fragment here
            //IWindowManager wm = (IWindowManager)Activity.GetSystemService(Context.WindowService);
            //int wmHeight = wm.DefaultDisplay.Height;
        }
        public static RecyclerViewFragment Instance(int position)
        {
            RecyclerViewFragment rf = new RecyclerViewFragment();
            Bundle b = new Bundle();
            b.PutInt("position",position);
            rf.Arguments = b;
            return rf;
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
             base.OnCreateView(inflater, container, savedInstanceState);
         //   recyclerView_foot = Activity.FindViewById<LinearLayout>(Resource.Id.recyclerView_foot);
            return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            InitRecyclerView();
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            //_recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
         
        }

        async void InitRecyclerView()
        {
            articleList = await SQLiteUtil.SelectArticleList(Constact.PageSize);
            if (articleList != null && articleList.Count > 0)
            {
                initRecycler();
                OnRefresh();
            }
            else
            {
                var result = await ArticleService.ListArticle(AccessTokenUtil.GetToken(this.Activity), pageIndex, position);
                if (result.Success)
                {
                    articleList = result.Data;
                    initRecycler();
                    if (lastRefreshTime.Year==0)
                    {
                        lastRefreshTime = DateTime.Now;
                        await SQLiteUtil.UpdateArticleList(articleList);
                    }
                    else if (lastRefreshTime.Subtract(DateTime.Now).Seconds > 60)
                    {
                        await SQLiteUtil.UpdateArticleList(articleList);
                    }
                }
                else
                {
                    AlertUtil.ToastShort(Activity, result.Message);
                }
                _swipeRefreshLayout.PostDelayed(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                },1000);
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
            var result = await ArticleService.ListArticle(AccessTokenUtil.GetToken(this.Activity), pageIndex, position);
            if (result.Success)
            {
                var tempList = result.Data;
                if (tempList == null || tempList.Count == 0)
                {
                    AlertUtil.ToastShort(Activity, "网络不太好哦");
                }
                else
                {
                    // Thread.Sleep(2000);
                    articleList.AddRange(tempList);
                    adapter.SetNewData(articleList);
                    adapter.NotifyItemRemoved(adapter.ItemCount);
                    System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + articleList.Count);
                }
            }
            else
            {
                AlertUtil.ToastShort(Activity, "网络不太好哦");
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<Article>(this.Activity, articleList, Resource.Layout.item_recyclerview_article, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                    System.Diagnostics.Debug.Write(position, tag);
                    AlertUtil.ToastShort(this.Activity, tag);
                    var intent = new Intent(Activity, typeof(DetailArticleActivity));
                    intent.PutExtra("id", int.Parse(tag));
                    StartActivity(intent);
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                AlertUtil.ToastShort(this.Activity, tag);
            };
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);
         
                adapter.OnConvertView += (holder, position) =>
                {
                    holder.SetText(Resource.Id.tv_author, articleList[position].Author);
                    holder.SetText(Resource.Id.tv_postDate, articleList[position].PostDate.ToCommonString());
                    holder.SetText(Resource.Id.tv_viewCount, articleList[position].ViewCount + " " + read);
                    holder.SetText(Resource.Id.tv_commentCount, articleList[position].CommentCount + " " + comment);
                    holder.SetText(Resource.Id.tv_description, articleList[position].Description);
                    holder.SetText(Resource.Id.tv_diggCount, articleList[position].Diggcount + " " + digg);
                    holder.SetText(Resource.Id.tv_title, articleList[position].Title);
                    //holder.SetTag(Resource.Id.ly_item, articleList[position].Id.ToString());
                    holder.GetView<CardView>(Resource.Id.ly_item).Tag=articleList[position].Id.ToString();
                    holder.SetTagUrl(Resource.Id.iv_avatar, articleList[position].Avatar);
                    holder.SetImageLoader(Resource.Id.iv_avatar, options, articleList[position].Avatar);
                };
        }

        public async void OnRefresh()
        {
            if(pageIndex>1)
                pageIndex = 1; 
            var result  = await ArticleService.ListArticle(AccessTokenUtil.GetToken(this.Activity), pageIndex, position);
            if (result.Success)
            {
                var tempList = result.Data;
                if (tempList != null && tempList.Count > 0)
                {
                    articleList = tempList;
                    adapter.SetNewData(tempList);
                    //lastRefreshTime 避免短时间内刷新，多次插入sqlite。
                    if (lastRefreshTime.Ticks==0)
                    {
                        lastRefreshTime = DateTime.Now;
                        await SQLiteUtil.UpdateArticleList(tempList);
                    }
                    else if (lastRefreshTime.Subtract(DateTime.Now).Seconds > 60)
                    {
                        await SQLiteUtil.UpdateArticleList(tempList);
                    }
                }
                else
                {
                    AlertUtil.ToastShort(Activity, "网络不太好哦");
                }
            }
            else
            {
                AlertUtil.ToastShort(Activity,result.Message);
            }
            _swipeRefreshLayout.PostDelayed(() =>
            {
                _swipeRefreshLayout.Refreshing = false;
            }, 1000);
        }
    }
}
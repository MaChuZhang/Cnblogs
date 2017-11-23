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
            _swipeRefreshLayout.PostDelayed(() =>
            {
                System.Diagnostics.Debug.Write("PostDelayed刷新已经完成");
                _swipeRefreshLayout.Refreshing = false;
            },3000);
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            //_recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
            articleList = await listArticleLocal();
            if (articleList != null && articleList.Count > 0)
            {
                initRecycler();
            }
            else
            {
                articleList = await  listArticleServer();
                initRecycler();
            }
            articleList = await listArticleServer();
            System.Diagnostics.Debug.Write("刷新已经完成");
            initRecycler();
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listArticleServer();
            articleList.AddRange(tempList);
            if (tempList.Count==0)
             {
                return;
            }
            else if (articleList != null)
            {
               // Thread.Sleep(2000);
                adapter.SetNewData(articleList);
                adapter.NotifyItemRemoved(adapter.ItemCount);
                System.Diagnostics.Debug.Write("页数:"+pageIndex+"数据总条数："+articleList.Count);
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
        private async Task<List<Article>> listArticleServer()
        {
            var result = await ArticleRequest.GetArticleList(AccessTokenUtil.GetToken(this.Activity),pageIndex,position);
            if (result.Success)
            {
                try
                {
                    await SQLiteUtil.UpdateArticleList(result.Data);
                    return result.Data;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.ToString());
                    return null;
                }
            }
            _swipeRefreshLayout.Refreshing = false;
            return null;
        }
        private async Task<List<Article>> listArticleLocal()
        {
            articleList = await SQLiteUtil.SelectArticleList(Constact.PageSize);
            return articleList;
        }

        public async void OnRefresh()
        {
            if(pageIndex>1)
                pageIndex = 1; 
            var tempList  = await listArticleServer();
            if (tempList != null)
            {
                articleList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }
    }
}
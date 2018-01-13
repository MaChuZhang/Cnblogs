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
        private Token accessToken;
        private bool isFirstRefresh=true;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .CacheOnDisk(true)
                 // .Displayer(new DisplayerImageCircle(20))
                  .Build();
            accessToken = AccessTokenUtil.GetToken(this.Activity);

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
             base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
                _swipeRefreshLayout.SetOnRefreshListener(this);
                _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
                _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
                articleList = await SQLiteUtil.SelectArticleList(Constact.PageSize);
                    if (articleList != null && articleList.Count > 0)
                    {
                        initRecycler();
                    }
                    OnRefresh();
                //_recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
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
                    await SQLiteUtil.UpdateArticleList(tempList);
                    System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + articleList.Count);
                }
            }
            else
            {
                AlertUtil.ToastShort(Activity,result.Message);
            }
        }
        void initRecycler()
        {
            try
            {
                adapter = new BaseRecyclerViewAdapter<Article>(this.Activity, articleList, Resource.Layout.item_recyclerview_article, LoadMore);
                _recyclerView.SetAdapter(adapter);
                adapter.ItemClick += (position, tag) =>
                {
                    System.Diagnostics.Debug.Write(position, tag);
                    //AlertUtil.ToastShort(this.Activity, tag);
                    var intent = new Intent(Activity, typeof(DetailBlogActivity));
                    intent.PutExtra("id", int.Parse(tag));
                    StartActivity(intent);
                };
                adapter.ItemLongClick += (tag, position) =>
                {
                    //AlertUtil.ToastShort(this.Activity, tag);
                };
                adapter.OnConvertView += (holder, position) =>
                {
                    holder.SetText(Resource.Id.tv_author, articleList[position].Author);
                    holder.SetText(Resource.Id.tv_postDate, articleList[position].PostDate.ToCommonString());
                    holder.SetText(Resource.Id.tv_viewCount, articleList[position].ViewCount.ToString());
                    holder.SetText(Resource.Id.tv_commentCount, articleList[position].CommentCount.ToString());
                    holder.SetText(Resource.Id.tv_description, articleList[position].Description);
                    holder.SetText(Resource.Id.tv_diggCount, articleList[position].Diggcount.ToString());
                    holder.SetText(Resource.Id.tv_title, articleList[position].Title);
                    //holder.SetTag(Resource.Id.ly_item, articleList[position].Id.ToString());
                    holder.GetView<CardView>(Resource.Id.ly_item).Tag = articleList[position].Id.ToString();
                    holder.SetTagUrl(Resource.Id.iv_avatar, articleList[position].Avatar);
                    holder.SetImageLoader(Resource.Id.iv_avatar, options, articleList[position].Avatar);
                };
            }
            catch (Exception ex)
            {

            }
        }

        public async void OnRefresh()
        {
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            pageIndex = 1; 
            var result  = await ArticleService.ListArticle(accessToken, pageIndex, position);
            if (result.Success)
            {
                var tempList = result.Data;
                if (tempList != null && tempList.Count > 0)
                {
                    articleList = tempList;
                    if (isFirstRefresh)
                    {
                        initRecycler();
                    }
                    else
                    {
                        adapter.SetNewData(tempList);
                        isFirstRefresh = false;
                    }
                    await SQLiteUtil.UpdateArticleList(tempList);
                }
                else
                {
                    AlertUtil.ToastShort(Activity, "网络不太好哦");
                }
                _swipeRefreshLayout.PostDelayed(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                },800);
            }
            else
            {
                AlertUtil.ToastShort(Activity,result.Message);
                _swipeRefreshLayout.PostDelayed(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                },800);
            }

        }
    }
}
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
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            // Create your fragment here
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
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));

            articleList = await listArticleServer(1);
            
            //articleList = await listArticleLocal();
            if (articleList != null)
            {
                initRecycler();
            }
            RecyclerView.OnScrollListener scroll = new RecyclerViewOnScrollListtener(_swipeRefreshLayout,(Android.Support.V7.Widget.LinearLayoutManager)_recyclerView.GetLayoutManager(), adapter, LoadMore);
            _recyclerView.AddOnScrollListener(scroll);
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listArticleServer(pageIndex);
            articleList.AddRange(tempList);
            if (tempList.Count==0)
             {
                //adapter.SetFooterView(Resource.Layout.item_recyclerView_footer_empty);
                return;
            }
            else if (articleList != null)
            {
                //adapter.NotifyDataSetChanged();
               // adapter.NotifyItemChanged(articleList.Count + 1);
                //var ds= adapter.ItemCount;
               adapter.SetNewData(articleList);
                System.Diagnostics.Debug.Write("页数:"+pageIndex+"数据总条数："+articleList.Count);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<Article>(this.Activity, articleList, Resource.Layout.item_recyclerview_article);
          //  View  footerView = LayoutInflater.From(Activity).Inflate(Resource.Layout.item_recyclerView_footer_loading, null);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                System.Diagnostics.Debug.Write(position, tag);
                AlertUtil.ToastShort(this.Activity, tag);
                var intent = new Intent(Activity, typeof(DetailArticleActivity));
                intent.PutExtra("id", int.Parse(tag));
                StartActivity(intent);
            };
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);
         
                adapter.OnConvertView += (holder, position) =>
                {
                    //if (position >= articleList.Count)
                    //    return;
                    holder.SetText(Resource.Id.tv_author, articleList[position].Author);
                    holder.SetText(Resource.Id.tv_postDate, articleList[position].PostDate.ToCommonString());
                    holder.SetText(Resource.Id.tv_viewCount, articleList[position].ViewCount + " " + read);
                    holder.SetText(Resource.Id.tv_commentCount, articleList[position].CommentCount + " " + comment);
                    holder.SetText(Resource.Id.tv_description, articleList[position].Description);
                    holder.SetText(Resource.Id.tv_diggCount, articleList[position].Diggcount + " " + digg);
                    holder.SetText(Resource.Id.tv_title, articleList[position].Title);
                    holder.SetTag(Resource.Id.ly_item, articleList[position].Id.ToString());
                    holder.SetTagUrl(Resource.Id.iv_avatar, articleList[position].Avatar);
                     //ImageLoader.Instance.DisplayImage(articleList[position].Avatar, Resource.Id.iv_avatar);
                holder.SetImageLoader(Resource.Id.iv_avatar, options);
                };
        }
        private async Task<List<Article>> listArticleServer(int _pageIndex)
        {
            pageIndex = _pageIndex;
            var result = await ArticleRequest.GetArticleList(SharedDataUtil.GetToken(this.Activity),pageIndex,position);
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                //articleList = result.Data;
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
            return null;
        }
        private async Task<List<Article>> listArticleLocal()
        {
            articleList = await SQLiteUtil.SelectArticleList(10);
            return articleList;
        }

        public async void OnRefresh()
        {
            if(pageIndex>1)
                pageIndex = 1; 
            var tempList  = await listArticleServer(pageIndex);
            if (tempList != null)
            {
                articleList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }
        //void ConvertView(BaseHolder holder,int position)
        //{
        //    holder.SetText();
        //}
    }
}
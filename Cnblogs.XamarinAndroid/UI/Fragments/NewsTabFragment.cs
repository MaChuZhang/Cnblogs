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
    public class NewsTabFragment : Fragment,SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<NewsViewModel> adapter;
        private LinearLayout ly_expire;
        private TextView tv_startLogin;
        public int position;
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private List<NewsViewModel> newsList = new List<NewsViewModel>();
        private Button btn_status;
        private Button btn_question;
        private bool isMy; 
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            isMy = Arguments.GetBoolean("isMy");
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  //.Displayer(new DisplayerImageCircle(20))
                  .Build();
        }
        public static NewsTabFragment Instance(int position)
        {
            NewsTabFragment rf = new NewsTabFragment();
            Bundle b = new Bundle();
            b.PutInt("position",position);
            rf.Arguments = b;
            return rf;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
             base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ly_expire = view.FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = view.FindViewById<TextView>(Resource.Id.tv_startLogin);
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
            }, 4000);
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            //_recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
            Token token = UserTokenUtil.GetToken(Activity);

            newsList = await listNewsLocal();
            if (newsList != null&&newsList.Count>0)
            {
                initRecycler();
            }
            else
            {
                newsList = await listNewsServer();
                if (newsList != null && newsList.Count > 0)
                {
                    initRecycler();
                }
            }
            newsList = await listNewsServer();
            if (newsList != null && newsList.Count > 0)
            {
                initRecycler();
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listNewsServer();
            newsList.AddRange(tempList);
            if (tempList.Count==0)
             {
                  return;
            }
            else if (newsList != null&&newsList.Count>0)
            {
               adapter.SetNewData(newsList);
                System.Diagnostics.Debug.Write("页数:"+pageIndex+"数据总条数："+newsList.Count);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<NewsViewModel>(this.Activity, newsList, Resource.Layout.item_recyclerview_news, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                    System.Diagnostics.Debug.Write(position, tag);
                    AlertUtil.ToastShort(this.Activity, tag);
                    DetailNewsActivity.Enter(Activity, int.Parse(tag));
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                AlertUtil.ToastShort(this.Activity, tag);
            };
            string comment = Resources.GetString(Resource.String.comment);
            string view = Resources.GetString(Resource.String.view);
            string digg = Resources.GetString(Resource.String.digg);
            adapter.OnConvertView += (holder, position) =>
                {
                    var model = newsList[position];
                    holder.SetText(Resource.Id.tv_description, model.Summary);
                    holder.SetText(Resource.Id.tv_title,model.Title);
                    holder.SetText(Resource.Id.tv_commentCount, comment+" "+model.CommentCount.ToString());
                    holder.SetText(Resource.Id.tv_dateAdded, model.DateAdded.ToCommonString());
                    holder.SetText(Resource.Id.tv_diggCount,digg+" " +model.DiggCount.ToString());
                    holder.SetText(Resource.Id.tv_viewCount,view+" "+ model.ViewCount.ToString());
                    holder.GetView<CardView>(Resource.Id.ly_item).Tag = model.Id.ToString();
                    holder.SetImageLoader(Resource.Id.iv_topicIcon, options,model.TopicIcon);
                };
        }
        private async Task<List<NewsViewModel>> listNewsServer()
        {
             var   result = await NewsService.ListNews(AccessTokenUtil.GetToken(this.Activity), pageIndex,position);
            if (result.Success)
            { 
                _swipeRefreshLayout.Refreshing = false;
                var data = result.Data;
                try
                {
                    if (position == 1)
                    {
                        data.ForEach(f => f.IsRecommend = true);
                    }
                    if (position == 2)
                    {
                        data.ForEach(f => f.IsHot = true);
                    }
                    await SQLiteUtil.UpdateNewsList(data);
                    return data;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.ToString());
                    return null;
                }
            }
            return null;
        }
        private async Task<List<NewsViewModel>> listNewsLocal()
        {
            if (position == 1)
            {
                return await SQLiteUtil.SelectNewsListByDigg(Constact.PageSize);
            }
            if (position == 2)
            {
                return await SQLiteUtil.SelectNewsListByHotWeek(Constact.PageSize);
            }
            return await SQLiteUtil.SelectNewsList(Constact.PageSize);
        }

        public async void OnRefresh()
        {
            if(pageIndex>1)
                pageIndex = 1; 
            var tempList  = await listNewsServer();
            if (tempList != null)
            {
                newsList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }
    }
}
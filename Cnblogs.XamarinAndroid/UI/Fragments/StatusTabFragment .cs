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
    public class StatusTabFragment : Fragment,SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<StatusModel> adapter;
        public int position;
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private List<StatusModel> statusList = new List<StatusModel>();
        private Button btn_status;
        private Button btn_question;
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
        public static StatusTabFragment Instance(int position)
        {
            StatusTabFragment rf = new StatusTabFragment();
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

            statusList = await listStatusServer(pageIndex);
            
            //statusList = await listStatusLocal();
            if (statusList != null)
            {
                initRecycler();
            }
            RecyclerView.OnScrollListener scroll = new RecyclerViewOnScrollListtener(_swipeRefreshLayout,(Android.Support.V7.Widget.LinearLayoutManager)_recyclerView.GetLayoutManager(), adapter, LoadMore);
            _recyclerView.AddOnScrollListener(scroll);
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listStatusServer(pageIndex);
            statusList.AddRange(tempList);
            if (tempList.Count==0)
             {
                //adapter.SetFooterView(Resource.Layout.item_recyclerView_footer_empty);
                return;
            }
            else if (statusList != null)
            {
                //adapter.NotifyDataSetChanged();
               // adapter.NotifyItemChanged(statusList.Count + 1);
                //var ds= adapter.ItemCount;
               adapter.SetNewData(statusList);
                System.Diagnostics.Debug.Write("页数:"+pageIndex+"数据总条数："+statusList.Count);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<StatusModel>(this.Activity, statusList, Resource.Layout.item_recyclerview_status);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                System.Diagnostics.Debug.Write(position, tag);
                AlertUtil.ToastShort(this.Activity, tag);
                StatusesCommentActivity.Enter(Activity,int.Parse(tag));
            };
            string comment = Resources.GetString(Resource.String.comment);
                adapter.OnConvertView += (holder, position) =>
                {
                    //if (position >= statusList.Count)
                    //    return;
                    holder.SetText(Resource.Id.tv_commentCount, statusList[position].CommentCount.ToString());
                    holder.SetText(Resource.Id.tv_dateAdded, statusList[position].DateAdded.ToCommonString());
                    holder.SetText(Resource.Id.tv_content, statusList[position].Content);
                    holder.SetText(Resource.Id.tv_userDisplayName, statusList[position].UserDisplayName);
                    holder.SetTag(Resource.Id.ly_item, statusList[position].Id.ToString());
                     //ImageLoader.Instance.DisplayImage(statusList[position].Avatar, Resource.Id.iv_avatar);
                    holder.SetImageLoader(Resource.Id.iv_userIcon, options,statusList[position].UserIconUrl);
                };
        }
        private async Task<List<StatusModel>> listStatusServer(int _pageIndex)
        {
            pageIndex = _pageIndex;
            var result = await StatusRequest.ListStatus(AccessTokenUtil.GetToken(this.Activity),position,pageIndex);
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                //statusList = result.Data;
                try
                {
                    await SQLiteUtil.UpdateStatusList(result.Data);
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
        private async Task<List<StatusModel>> listStatusLocal()
        {
            statusList = await SQLiteUtil.SelectStatusList(Constact.PageSize);
            return statusList;
        }

        public async void OnRefresh()
        {
            if(pageIndex>1)
                pageIndex = 1; 
            var tempList  = await listStatusServer(pageIndex);
            if (tempList != null)
            {
                statusList = tempList;
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
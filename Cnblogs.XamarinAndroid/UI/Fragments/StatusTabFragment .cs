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
        private LinearLayout ly_expire;
        private TextView tv_startLogin;
        public int position;
        private DisplayImageOptions options;
        private int pageIndex = 1;
        private List<StatusModel> statusList = new List<StatusModel>();
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
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
        }
        public static StatusTabFragment Instance(int position)
        {
            StatusTabFragment rf = new StatusTabFragment();
            Bundle b = new Bundle();
            b.PutInt("position",position);
            rf.Arguments = b;
            return rf;
        }

        public static StatusTabFragment Instance(int position,bool isMy)
        {
            StatusTabFragment rf = new StatusTabFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            b.PutBoolean("isMy", isMy);
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
            if (isMy && token.IsExpire)
            {
                ly_expire.Visibility = ViewStates.Visible;
                _swipeRefreshLayout.Visibility = ViewStates.Gone;
                tv_startLogin.Click += (s, e) =>
                {
                    Activity.StartActivity(new Intent(Activity,typeof(loginactivity)));
                };
                return;
            }
            else
            {
                ly_expire.Visibility = ViewStates.Gone;
                _swipeRefreshLayout.Visibility = ViewStates.Visible;
            }
            statusList = await listStatusLocal();
            if (statusList != null)
            {
                initRecycler();
            }
            else
            {
                statusList = await listStatusServer();
                if (statusList != null)
                {
                    initRecycler();
                }
            }
            statusList = await listStatusServer();
            if (statusList != null)
            {
                initRecycler();
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listStatusServer();
            statusList.AddRange(tempList);
            if (tempList.Count==0)
             {
                  return;
            }
            else if (statusList != null)
            {
               adapter.SetNewData(statusList);
                System.Diagnostics.Debug.Write("页数:"+pageIndex+"数据总条数："+statusList.Count);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<StatusModel>(this.Activity, statusList, Resource.Layout.item_recyclerview_status, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
            
                    System.Diagnostics.Debug.Write(position, tag);
                    AlertUtil.ToastShort(this.Activity, tag);
                    StatusesCommentActivity.Enter(Activity, int.Parse(tag));
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                AlertUtil.ToastShort(this.Activity, tag);
            };
            string comment = Resources.GetString(Resource.String.comment);
                adapter.OnConvertView += (holder, position) =>
                {
                    //if (position >= statusList.Count)
                    //    return;
                    holder.SetText(Resource.Id.tv_commentCount, statusList[position].CommentCount.ToString());
                    holder.SetText(Resource.Id.tv_dateAdded, statusList[position].DateAdded.ToCommonString());
                    (holder.GetView<TextView>(Resource.Id.tv_content)).SetText(HtmlUtil.GetHtml(statusList[position].Content), TextView.BufferType.Spannable);
                    holder.SetText(Resource.Id.tv_userDisplayName, statusList[position].UserDisplayName);
                    holder.GetView<CardView>(Resource.Id.ly_item).Tag = statusList[position].Id.ToString();
                    holder.SetImageLoader(Resource.Id.iv_userIcon, options,statusList[position].UserIconUrl);
                };
        }
        private async Task<List<StatusModel>> listStatusServer()
        {
            var result = new ApiResult<List<StatusModel>>();
            if (isMy)
                result = await StatusRequest.ListStatus(UserTokenUtil.GetToken(this.Activity), position, pageIndex, true);
            else
            {
                if (position == 0)
                {
                    result = await StatusRequest.ListStatus(AccessTokenUtil.GetToken(this.Activity), position, pageIndex, false);
                }
                else
                    result = await StatusRequest.ListStatus(UserTokenUtil.GetToken(this.Activity), position, pageIndex, false);
            }
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
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
            var tempList  = await listStatusServer();
            if (tempList != null)
            {
                statusList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }
    }
}
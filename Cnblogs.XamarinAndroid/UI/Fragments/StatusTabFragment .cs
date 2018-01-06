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
        private bool isMy;
        private Token userToken,accessToken;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            isMy = Arguments.GetBoolean("isMy");
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .CacheOnDisk(true)
                  //.Displayer(new DisplayerImageCircle(20))
                  .Build();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
             base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_recyclerview,container,false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ly_expire = view.FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = view.FindViewById<TextView>(Resource.Id.tv_startLogin);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            userToken = UserTokenUtil.GetToken(Activity);
            accessToken = AccessTokenUtil.GetToken(Activity);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            //_recyclerView.AddItemDecoration(new RecyclerViewDecoration(this.Activity, (int)Orientation.Vertical));
            try
            {
                if (position !=0 || isMy) //除了最新闪存，其他的都要usertoken
                {
                    if (UserUtil.Instance(Activity).LoginExpire())
                    {
                        ly_expire.Visibility = ViewStates.Visible;
                        _swipeRefreshLayout.Visibility = ViewStates.Gone;
                        tv_startLogin.Click += (s, e) =>
                        {
                            Activity.StartActivity(new Intent(Activity, typeof(loginactivity)));
                        };
                    }
                    else
                    {
                        ly_expire.Visibility = ViewStates.Gone;
                        _swipeRefreshLayout.Visibility = ViewStates.Visible;
                        OnRefresh();
                    }
                }
                else
                {
                    ly_expire.Visibility = ViewStates.Gone;
                    _swipeRefreshLayout.Visibility = ViewStates.Visible;
                    OnRefresh();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("statusTabFragment",ex.ToString());
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
            var result = new ApiResult<List<StatusModel>>();
            if (isMy)
                result = await StatusService.ListStatus(UserTokenUtil.GetToken(this.Activity), position, pageIndex, true);
            else
            {
                if (position == 0)
                {
                    result = await StatusService.ListStatus(AccessTokenUtil.GetToken(this.Activity), position, pageIndex, false);
                }
                else
                    result = await StatusService.ListStatus(UserTokenUtil.GetToken(this.Activity), position, pageIndex, false);
            }
            if (result.Success)
            {
                var tempList = result.Data;
                statusList.AddRange(tempList);
                adapter.SetNewData(statusList);
            }
            else 
            {
                AlertUtil.ToastShort(Activity,result.Message);
            }
        }
         void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<StatusModel>(this.Activity, statusList, Resource.Layout.item_recyclerview_status, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
            
                    System.Diagnostics.Debug.Write(position, tag);
                    //AlertUtil.ToastShort(this.Activity, tag);
                    StatusesCommentActivity.Enter(Activity, int.Parse(tag));
            };
            adapter.ItemLongClick += (tag, _position) =>
            {
                if (Activity is MyStatusActivity&&position==0)
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
                    builder.SetCancelable(true);
                    string[] btns = Resources.GetStringArray(Resource.Array.DialogDelete);
                    var model = statusList.Find(f => f.Id == int.Parse(tag));
                    builder.SetItems(btns, (s, e) =>
                    {
                        if (e.Which == 0)
                        {
                            Delete(model);
                        }
                    }).Show();
                   //AlertUtil.ToastShort(this.Activity, tag);
                }
            };
            string comment = Resources.GetString(Resource.String.comment);
                adapter.OnConvertView += (holder, position) =>
                {
                    //if (position >= statusList.Count)
                    //    return;
                    var model = statusList[position];
                    holder.SetText(Resource.Id.tv_commentCount,model.CommentCount.ToString());
                    holder.SetText(Resource.Id.tv_dateAdded, model.DateAdded.ToCommonString());
                    (holder.GetView<TextView>(Resource.Id.tv_content)).SetText(HtmlUtil.GetHtml(model.Content), TextView.BufferType.Spannable);
                    holder.SetText(Resource.Id.tv_userDisplayName, model.UserDisplayName);
                    holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = model.Id.ToString();
                    holder.GetView<ImageView>(Resource.Id.iv_userIcon).Tag=(model.UserIconUrl);
                    holder.SetImageLoader(Resource.Id.iv_userIcon, options, model.UserIconUrl);
                };
        }
        /// <summary>
        /// 删除我的闪存
        /// </summary>
        /// <param name="model"></param>
        private void Delete(StatusModel model)
        {
            ProgressDialog progressDialog = new ProgressDialog(Activity);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("删除中....");
            progressDialog.Show();
            StatusService.Delete(UserTokenUtil.GetToken(Activity),model.Id,(success)=> {
                Activity.RunOnUiThread(() =>
                {
                    progressDialog.Hide();
                    statusList.Remove(model);
                    adapter.SetNewData(statusList);
                });
            },(error)=> {
                Activity.RunOnUiThread(() =>
                {
                    progressDialog.Hide();
                    AlertUtil.ToastShort(Activity, error);
                });
            });
        }

        public async void OnRefresh()
        {
            try
            {
                pageIndex = 1;
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = true;
                });
                var result = new ApiResult<List<StatusModel>>();
                if (isMy)
                    result = await StatusService.ListStatus(userToken, position, pageIndex, true);
                else
                {
                    if (position !=0)
                    {
                        result = await StatusService.ListStatus(UserTokenUtil.GetToken(Activity), position, pageIndex, false);
                    }
                    else
                        result = await StatusService.ListStatus(accessToken, position, pageIndex, false);
                }
                if (result.Success)
                {
                    statusList = result.Data;
                    initRecycler();
                    if (statusList.Count != 0)
                    {
                        await SQLiteUtil.UpdateStatusList(statusList);
                    }
                }
                else {
                    AlertUtil.ToastShort(Activity,result.Message);
                }
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
            catch (Exception ex)
            {

            }
        }

        public static StatusTabFragment Instance(int position)
        {
            StatusTabFragment rf = new StatusTabFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            rf.Arguments = b;
            return rf;
        }

        public static StatusTabFragment Instance(int position, bool isMy)
        {
            StatusTabFragment rf = new StatusTabFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            b.PutBoolean("isMy", isMy);
            rf.Arguments = b;
            return rf;
        }
    }
}
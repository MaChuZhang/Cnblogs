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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Webkit;
using Android.Support.V4.App;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;
using Cnblogs.XamarinAndroid;
using Cnblogs.ApiModel;
using Com.Umeng.Socialize;
using Cnblogs.XamarinAndroid.UI.Widgets;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Cnblogs.XamarinAndroid.UI;
using Cnblogs.HttpClient;
using System.Threading.Tasks;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "StatusesCommentActivity", Theme = "@style/AppTheme")]
    public class StatusesCommentActivity : BaseActivity,View.IOnClickListener
    {
        //private Toolbar toolbar;
        private TextView tv_userDisplayName, tv_dateAdded, tv_content;
        private ImageView iv_userIcon;
        private int statusId;
        private DisplayImageOptions options;
        private EditText edit_content;
        private Button btn_submit;
        private StatusModel status;
        private UMengShareWidget shareWidget;

        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<StatusCommentsModel> adapter;
        private List<StatusCommentsModel> listStatusComment;
        private int parentCommentId, replyToUserId;
        private string atUserName,displayUserName;
        protected override int LayoutResourceId => Resource.Layout.statusesComment;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.comment);

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//初始化图片加载框架
            ImageLoader.Instance.Init(configuration);
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_loading)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            SetToolBarNavBack();
            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorScheme(Resource.Color.primary);
            tv_dateAdded = FindViewById<TextView>(Resource.Id.tv_dateAdded);
            tv_userDisplayName = FindViewById<TextView>(Resource.Id.tv_userDisplayName);
            iv_userIcon = FindViewById<ImageView>(Resource.Id.iv_userIcon);
            tv_content = FindViewById<TextView>(Resource.Id.tv_content);
            edit_content = FindViewById<EditText>(Resource.Id.edit_content);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            btn_submit.Click += AddCommentClick;
            statusId = Intent.GetIntExtra("id", 0);
            GetClientStatus(statusId);
            shareWidget = new UMengShareWidget(this);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));


            _swipeRefreshLayout.Post(()=> {
                _swipeRefreshLayout.Refreshing = true;
            });
            _swipeRefreshLayout.Refresh += async (s, e) =>
            {
                await StatusService.ListStatusComment(AccessTokenUtil.GetToken(this), statusId, callBackSuccess, callBackError);
            };
            edit_content.TextChanged += (s, e) =>
            {
                if (string.IsNullOrEmpty(edit_content.Text.Trim()))
                {
                    btn_submit.Enabled = false;
                }
                else
                    btn_submit.Enabled = true;
            };
            AlertUtil.ToastShort(this,"线程ID"+System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
           await StatusService.ListStatusComment(AccessTokenUtil.GetToken(this), statusId, callBackSuccess, callBackError);
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this, (int)Orientation.Vertical));
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share,menu);
            return  true;
        }
        internal static void Enter(Context context,int id)
        {
            Intent intent = new Intent(context,typeof(StatusesCommentActivity));
            intent.PutExtra("id",id);
            context.StartActivity(intent);
        }
        async void GetClientStatus(int id)
        {
            try
            {
                status = await SQLiteUtil.SelectStatus(id);
                if (status != null)
                {
                    tv_userDisplayName.Text = status.UserDisplayName;
                    tv_dateAdded.Text = status.DateAdded.ToCommonString();
                    tv_content.Text = status.Content;
                    if (!status.UserIconUrl.Substring(status.UserIconUrl.Length - 4, 4).Contains(".png"))
                        iv_userIcon.SetImageResource(Resource.Drawable.noavatar);
                    else
                        ImageLoader.Instance.DisplayImage(status.UserIconUrl, iv_userIcon, options);
                }
            }
            catch (Exception ex)
            {

            }
        }
        void initRecycler()
        {
            listStatusComment = listStatusComment.OrderByDescending(s => s.DateAdded).ToList();
            adapter = new BaseRecyclerViewAdapter<StatusCommentsModel>(this, listStatusComment, Resource.Layout.item_recyclerview_statusComment, ()=> { });
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
             {
                 var currentCommentModel = listStatusComment.Find(p => p.Id == int.Parse(tag));
                 var userInfo = UserInfoShared.GetUserInfo(this);
                 if (userInfo.DisplayName == currentCommentModel.UserDisplayName)
                 {
                     return;
                 }
                 parentCommentId = currentCommentModel.Id;
                 replyToUserId = currentCommentModel.UserId;
                 atUserName = "<a href='http://home.cnblogs.com/u/"+currentCommentModel.UserId+"'>@"+currentCommentModel.UserDisplayName+"：</a>";
                 displayUserName = currentCommentModel.UserDisplayName;
                 edit_content.SetText(HtmlUtil.GetHtml(atUserName), TextView.BufferType.Spannable);
                // edit_content.Text = "@"+currentCommentModel.UserDisplayName;
             };
            adapter.ItemLongClick += (tag, position) =>
            {
                //删除我评论闪存
                var model = listStatusComment.Find(f=>f.Id==int.Parse(tag));
               var  user = UserInfoShared.GetUserInfo(this);
                if (model.UserDisplayName==user.DisplayName)
                {
                    DeleteComment(model);
                }
            };
            string read = Resources.GetString(Resource.String.read);
            string digg = Resources.GetString(Resource.String.digg);
            adapter.OnConvertView += (holder, position) =>
            {
                holder.SetText(Resource.Id.tv_commentDateAdded, listStatusComment[position].DateAdded.ToCommonString());
                (holder.GetView<TextView>(Resource.Id.tv_commentContent)).SetText(HtmlUtil.GetHtml(listStatusComment[position].Content), TextView.BufferType.Spannable);
                holder.SetImageLoader(Resource.Id.iv_commentUserIcon, options, listStatusComment[position].UserIconUrl);
                holder.SetText(Resource.Id.tv_commentUserName, listStatusComment[position].UserDisplayName);
                holder.SetText(Resource.Id.tv_floor, adapter.ItemCount - position-1 + "楼");
                holder.SetTag(Resource.Id.ly_item, listStatusComment[position].Id.ToString());
            };
        }
        //删除我的闪存评论
        void DeleteComment(StatusCommentsModel model)
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetCancelable(true);
            string[] btns = Resources.GetStringArray(Resource.Array.DialogDelete);
            builder.SetItems(btns, (s, e) =>
            {
                ProgressDialog progressDialog = new ProgressDialog(this);
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.SetMessage("删除中....");
                progressDialog.Show();
                StatusService.DeleteComment(UserTokenUtil.GetToken(this),model.StatusId.ToString(),model.Id.ToString(),()=> {
                    RunOnUiThread(() =>
                    {
                        progressDialog.Hide();
                        listStatusComment.Remove(model);
                        adapter.SetNewData(listStatusComment);
                        AlertUtil.ToastShort(this,"删除成功");
                    });
                },
                (error=> {
                    RunOnUiThread(() =>
                    {
                        progressDialog.Hide();
                        AlertUtil.ToastShort(this, error);
                    });
                }));
            }).Show();
        }

        //添加闪存评论事件
        public async void AddCommentClick(Object o, EventArgs e)
        {
            string content = edit_content.Text.Trim();
            if (string.IsNullOrEmpty(content))
                return;
            if (!string.IsNullOrEmpty(displayUserName))
            {
                content = content.Replace(displayUserName, displayUserName + " ");
            }
            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("添加评论中....");
            progressDialog.Show();
            var result = await StatusService.AddComment(UserTokenUtil.GetToken(this), statusId.ToString(), replyToUserId,parentCommentId, content);
            if (result.Success)
            {
                RunOnUiThread(async () =>
                {
                    progressDialog.Hide();
                    await StatusService.ListStatusComment(AccessTokenUtil.GetToken(this), statusId, callBackSuccess, callBackError);
                    edit_content.Text = "";
                    AlertUtil.ToastShort(this, "评论成功");
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    progressDialog.Hide();
                    AlertUtil.ToastShort(this,result.Message);
                });
            }
        }
        //请求闪存评论成功的回调
        async void callBackSuccess(List<StatusCommentsModel> list)
        {
            AlertUtil.ToastShort(this, "线程ID" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            try
            {
                await SQLiteUtil.UpdateStatuscommentList(list);
            }
            catch (Exception ex)
            {

            }
            listStatusComment = list;
            initRecycler();
            _swipeRefreshLayout.PostDelayed(() =>
            {
                _swipeRefreshLayout.Refreshing = false;
            }, 500);
        }
        //请求闪存评论失败的回调
        void callBackError(string  error)
        {
            AlertUtil.ToastShort(this, "线程ID" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            _swipeRefreshLayout.PostDelayed(() =>
            {
                _swipeRefreshLayout.Refreshing = false;
            }, 1000);
            AlertUtil.ToastShort(this,"获取数据失败"+error);
        }

        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (status != null)
            {
                // sharesWidget
                //shareWidget.Open(status.Url,status.Title);
            }
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode,resultCode,data);
            UMShareAPI.Get(this).OnActivityResult(requestCode,(int)resultCode,data);
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}
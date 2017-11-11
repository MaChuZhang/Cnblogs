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
    public class StatusesCommentActivity : BaseActivity,SwipeRefreshLayout.IOnRefreshListener,View.IOnClickListener
    {
        //private Toolbar toolbar;
        private TextView tv_userDisplayName, tv_dateAdded, tv_content,tv_view,tv_commentContent, tv_commentUserName, tv_floor,tv_commentDateAdded;
        private ImageView iv_userIcon, iv_commentUserIcon;
        private int statusId;
        DisplayImageOptions options;

        private StatusModel status;
        private UMengShareWidget shareWidget;

        private RecyclerView _recyclerView;
        //private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<StatusCommentsModel> adapter;
        private List<StatusCommentsModel> listStatusComment;
        private int pageIndex = 1;
        protected override int LayoutResourceId => Resource.Layout.statusesComment;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.comment);

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//≥ı ºªØÕº∆¨º”‘ÿøÚº‹
            ImageLoader.Instance.Init(configuration);
            //œ‘ æÕº∆¨≈‰÷√
            options = new DisplayImageOptions.Builder()
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_loading)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            SetNavIcon(Resource.Drawable.icon_back);
            tv_dateAdded = FindViewById<TextView>(Resource.Id.tv_dateAdded);
            tv_userDisplayName = FindViewById<TextView>(Resource.Id.tv_userDisplayName);
            iv_userIcon = FindViewById<ImageView>(Resource.Id.iv_userIcon);
            tv_content = FindViewById<TextView>(Resource.Id.tv_content);
            //            tv_view = FindViewById<TextView>(Resource.Id.tv_view);

            statusId = Intent.GetIntExtra("id", 0);
            GetClientStatus(statusId);
            shareWidget = new UMengShareWidget(this);

            //_swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            //_swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            //_swipeRefreshLayout.SetOnRefreshListener(this);
            //_swipeRefreshLayout.Post(() =>
            //{
            //    _swipeRefreshLayout.Refreshing = true;

            //});
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            listStatusComment = await listStatusCommentsServer(pageIndex);

            //statusList = await listStatusLocal();
            if (listStatusComment != null)
            {
                initRecycler();
            }
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
        private async Task<List<StatusCommentsModel>> listStatusCommentsServer(int _pageIndex)
        {
            pageIndex = _pageIndex;
            var result = await  StatusRequest.ListStatusComment(AccessTokenUtil.GetToken(this), statusId);
            if (result.Success)
            {
               // _swipeRefreshLayout.Refreshing = false;
                try
                {

                    await SQLiteUtil.UpdateStatusCommentList(result.Data);
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

        async void initRecycler()
        {
            listStatusComment = listStatusComment.OrderByDescending(s => s.DateAdded).ToList();
            adapter = new BaseRecyclerViewAdapter<StatusCommentsModel>(this,listStatusComment, Resource.Layout.item_recyclerview_statusComment);
            _recyclerView.SetAdapter(adapter);
            //adapter.ItemClick += (position, tag) =>
            //{
            //    System.Diagnostics.Debug.Write(position, tag);
            //    AlertUtil.ToastShort(this, tag);
            //    DetailKbArticlesActivity.Enter(this, int.Parse(tag));
            //};
            adapter.ItemClick += (position, tag) =>
            {

            };
            string read = Resources.GetString(Resource.String.read);
            string digg = Resources.GetString(Resource.String.digg);
            adapter.OnConvertView += (holder, position) =>
            {
                holder.SetText(Resource.Id.tv_commentDateAdded, listStatusComment[position].DateAdded.ToCommonString());
                (holder.GetView<TextView>(Resource.Id.tv_commentContent)).SetText(HtmlUtil.GetHtml(listStatusComment[position].Content), TextView.BufferType.Spannable);
                holder.SetImageLoader(Resource.Id.iv_commentUserIcon,options,listStatusComment[position].UserIconUrl);
                holder.SetText(Resource.Id.tv_commentUserName, listStatusComment[position].UserDisplayName);
                holder.SetText(Resource.Id.tv_floor, adapter.ItemCount-position +"¬•");
                holder.SetTag(Resource.Id.ly_item, listStatusComment[position].Id.ToString());
            };
        }

        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            var tempList = await listStatusCommentServer(pageIndex);
            if (tempList != null)
            {
                listStatusComment = tempList;
               // _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }

        private async Task<List<StatusCommentsModel>> listStatusCommentServer(int _pageIndex)
        {
            pageIndex = _pageIndex;
            var result = await StatusRequest.ListStatusComment(AccessTokenUtil.GetToken(this), pageIndex);
            if (result.Success)
            {
                //_swipeRefreshLayout.Refreshing = false;
                try
                {
                    await SQLiteUtil.UpdateStatusCommentList(result.Data);
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
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
using Com.Nostra13.Universalimageloader.Core;
using Android.Support.V7.Widget;
using Cnblogs.ApiModel;
using Android.Support.V4.Widget;
using Android.Graphics;
using System.Threading.Tasks;
using Cnblogs.HttpClient;
using Cnblogs.XamarinAndroid.UI.Widgets;
using Com.Umeng.Analytics;
using System.Threading;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/comment",Theme ="@style/AppTheme")]
    public  class NewsCommentActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
    {
        
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.articleComment;
            }
        }

        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<NewsCommentViewModel> adapter;
        private DisplayImageOptions options;
        private int pageIndex = 1, POSTID,PARENTID;
        private EditText edit_content;
        private Button btn_submit;
        private string atUserName;
        private bool isAt,FIRSTREFRESH=true; 
        private List<NewsCommentViewModel> commentList = new List<NewsCommentViewModel>();
        private Token accessToken;
        internal static void Enter(Context context,int postId)
        {
            Intent intent = new Intent(context, typeof(NewsCommentActivity));
            intent.PutExtra("postId",postId);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            POSTID = Intent.GetIntExtra("postId",0);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.comment));
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

                edit_content = FindViewById<EditText>(Resource.Id.edit_content);
                btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
                //btn_submit.enabvle
                btn_submit.Click += (s, e) =>
                {
                    Add();
                };
                edit_content.TextChanged += (s, e) =>
                  {
                      string temp = edit_content.Text.TrimStart().TrimEnd();
                      if (!string.IsNullOrEmpty(temp))
                      {
                          btn_submit.Enabled = true;
                          if (atUserName != null && atUserName.Length > 0 && temp.Contains(atUserName))
                              isAt = true;
                          else
                              isAt = false;
                      }
                      else btn_submit.Enabled = false;
                  };

                _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
                _swipeRefreshLayout.SetOnRefreshListener(this);

                _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
                _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this));
                _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this, (int)Orientation.Vertical));
                System.Diagnostics.Debug.Write("主线程ID" + Thread.CurrentThread.ManagedThreadId);
                accessToken = AccessTokenUtil.GetToken(this);
                OnRefresh();
        }
        private async void Add()
        {
            ProgressDialog dialog = new ProgressDialog(this);
            dialog.SetTitle("评论");
            dialog.SetMessage("提交评论中.....");
            string body = edit_content.Text.TrimEnd().TrimStart();
            var userToken = UserTokenUtil.GetToken(this);
            if (userToken.IsExpire)
            {
                AlertDialog.Builder alertDialog = new AlertDialog.Builder(this)
                .SetTitle("登录提示")
                 .SetMessage("未登录或登录token已经过期")
                 .SetPositiveButton("授权", (s, e) =>
                 {
                     StartActivity(new Intent(this,typeof(loginactivity)));
                 })
                 .SetNegativeButton("取消", (s, e) =>
                 {
                     return;
                 });
                alertDialog.Create().Show();
            }
            if (string.IsNullOrEmpty(body))
            {
                AlertUtil.ToastShort(this, "请输入内容");
                return;
            }
            dialog.Show();
            var  result= await  NewsService.Add(userToken, PARENTID, POSTID, body);
            if (result.Success)
            {
                dialog.Hide();
                AlertUtil.ToastShort(this, "评论成功");
                edit_content.Text = "";
                btn_submit.Enabled = false;
                OnRefresh();
            }
            else
            {
                dialog.Hide();
                AlertUtil.ToastShort(this, result.Message);
                btn_submit.Enabled = true;
            }
        }
        private async void LoadMore()
        {
            pageIndex++;
            var result = await  NewsService.ListNewsComment(accessToken, POSTID, pageIndex);
            if (result.Success)
            {
                var tempList = result.Data;
                if (tempList != null && tempList.Count != 0)
                {
                    commentList.AddRange(tempList);
                    adapter.SetNewData(commentList);
                    System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + commentList.Count);
                }
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
            }
        }
        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<NewsCommentViewModel>(this, commentList, Resource.Layout.item_newsComment, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                    //System.Diagnostics.Debug.Write(position, tag);
                    //var intent = new Intent(this, typeof(DetailBlogActivity));
                    //intent.PutExtra("id", int.Parse(tag));
                    //StartActivity(intent);
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                if (!isAt)//目前仅支持at一次
                {
                    isAt = true;
                    PARENTID = commentList[position].ParentCommentID;
                    atUserName = commentList[position].UserName;
                    string temp = edit_content.Text;
                    edit_content.Text = "@" + atUserName + temp;
                    edit_content.SetSelection(edit_content.Text.Length);
                }
                //AlertUtil.ToastShort(this, tag);
            };
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                var model = commentList[position];
                holder.SetText(Resource.Id.tv_commentUserName, model.UserName);
                holder.SetText(Resource.Id.tv_ding, model.AgreeCount.ToString());
                holder.SetText(Resource.Id.tv_cai, model.AntiCount.ToString());
                holder.SetText(Resource.Id.tv_commentUserName, model.UserName);
                holder.SetText(Resource.Id.tv_commentDateAdded, model.DateAdded.ToCommonString());
                (holder.GetView<TextView>(Resource.Id.tv_commentContent)).SetText(HtmlUtil.GetHtml(model.CommentContent), TextView.BufferType.Spannable);
                holder.SetText(Resource.Id.tv_floor, model.Floor+"楼");
                holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = model.CommentID.ToString();
                holder.SetImageLoader(Resource.Id.iv_commentUserIcon, options, model.FaceUrl);
            };
        }
 

        public async void OnRefresh()
        {
            pageIndex = 1;
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            var  result= await NewsService.ListNewsComment(accessToken, POSTID, pageIndex);
            if (result.Success)
            {
                var tempList = result.Data;
                if (FIRSTREFRESH)
                {
                    commentList = tempList;
                    initRecycler();
                }
                else
                {
                    if (tempList != null && tempList.Count != 0)
                    {
                        commentList = tempList;
                        adapter.SetNewData(tempList);
                    }
                
                }
                FIRSTREFRESH = false;
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
                _swipeRefreshLayout.Post(() =>
                {
                    _swipeRefreshLayout.Refreshing = false;
                });
            }
        }
    }
}
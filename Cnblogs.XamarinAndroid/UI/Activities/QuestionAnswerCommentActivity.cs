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
namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/comment",Theme ="@style/AppTheme")]
    public  class QuestionAnswerCommentActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
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
        private BaseRecyclerViewAdapter<QuestionCommentViewModel> adapter;
        public string   blogApp; //博客名
        private DisplayImageOptions options;
        private int answerId,questionId,parentCommentId;
        private EditText edit_content;
        private Button btn_submit;
        private bool isAt;
        private string atUserName;
        private List<QuestionCommentViewModel> answerCommentList = new List<QuestionCommentViewModel>();
        private Token userToken;
        private UserInfo userInfo;
        internal static void Enter(Context context,int questionId, int answerId)
        {
            Intent intent = new Intent(context, typeof(QuestionAnswerCommentActivity));
            intent.PutExtra("answerId", answerId);
            intent.PutExtra("questionId", questionId);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            answerId = Intent.GetIntExtra("answerId", 0);
            questionId = Intent.GetIntExtra("questionId", 0);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle("回答评论");
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                  .ShowImageForEmptyUri(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            edit_content = FindViewById<EditText>(Resource.Id.edit_content);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            userToken = UserTokenUtil.GetToken(this);
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
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this));
            _recyclerView.AddItemDecoration(new RecyclerViewDecoration(this, (int)Orientation.Vertical));
            initRecyclerView();
            userInfo = UserInfoShared.GetUserInfo(this);
        }
        private async void initRecyclerView()
        {
            var result = await QuestionRequest.ListQuestionAnswerComment(AccessTokenUtil.GetToken(this), answerId);
            if (result.Success)
            {
                answerCommentList = result.Data;
                initRecycler();
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
            }
            _swipeRefreshLayout.PostDelayed(() =>
            {
                _swipeRefreshLayout.Refreshing = false;
            }, 1000);
        }
        private async void Add()
        {
            string body = edit_content.Text.TrimEnd().TrimStart();
            var userToken = UserTokenUtil.GetToken(this);
            var userInfo = UserInfoShared.GetUserInfo(this);
            if (userToken.IsExpire)
            {
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle("登录提示")
                 .SetMessage("未登录或登录token已经过期")
                 .SetPositiveButton("授权", (s1, e1) =>
                 {
                     StartActivity(new Intent(this, typeof(loginactivity)));
                 })
                 .SetNegativeButton("取消", (s1, e1) =>
                 {
                     return;
                 });
                alertDialog.Create().Show();
            }
            else
            {
                if (string.IsNullOrEmpty(body))
                {
                    AlertUtil.ToastShort(this, "请输入内容");
                    return;
                }
                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetTitle("评论");
                dialog.SetMessage("提交评论中.....");
                dialog.Show();
                var result = await QuestionRequest.AddQuestionAnswerComment(userToken, body, userInfo.BlogApp, questionId, answerId, parentCommentId, userInfo.SpaceUserId);
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
        }
        private async void LoadMore()
        {
            var result = await QuestionRequest.ListQuestionAnswerComment(userToken, answerId);
            if (result.Success)
            {
                var tempList = result.Data;
                answerCommentList.AddRange(tempList);
                adapter.SetNewData(answerCommentList);
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
            }
        }
         void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<QuestionCommentViewModel>(this, answerCommentList, Resource.Layout.item_recyclerview_questionAnswerComment, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position,tag)=> {
                parentCommentId = int.Parse(tag);
                atUserName = "<a href='#'>"+"@"+answerCommentList[position].PostUserName+"：" +"</a>";
                edit_content.SetText(HtmlUtil.GetHtml(atUserName), TextView.BufferType.Spannable);
            };
            adapter.ItemLongClick += ItemLongClick;
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                var model = answerCommentList[position];
                holder.SetText(Resource.Id.tv_commentUserName,model.PostUserName);
                holder.SetText(Resource.Id.tv_commentDateAdded, model.DateAdded.ToCommonString());
                if (!string.IsNullOrEmpty(model.Content.Trim()))
                {
                    (holder.GetView<TextView>(Resource.Id.tv_commentContent)).SetText(HtmlUtil.GetHtml(model.Content.Trim()), TextView.BufferType.Spannable);
                }
                holder.SetText(Resource.Id.tv_ding,model.DiggCount.ToString());
                holder.SetText(Resource.Id.tv_cai, model.BuryCount.ToString());
                holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = model.CommentID.ToString();
                //holder.SetText(Resource.Id.tv_commentCount, model..ToString());
                //holder.SetText(Resource.Id.tv_scoreName, HtmlUtil.GetScoreName(model));

                if (model.PostUserInfo != null)
                {
                    holder.SetImageLoader(Resource.Id.iv_commentUserIcon, options, Constact.CnblogsPic +model.PostUserInfo.IconName);
            
                }
            };
        }

        void ItemLongClick(string  tag, int position)
        {
            if (answerCommentList[position].PostUserName != userInfo.DisplayName)
                return;
            if (userToken.IsExpire)
            {
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle("登录提示")
                 .SetMessage("未登录或登录token已经过期")
                 .SetPositiveButton("授权", (s1, e1) =>
                 {
                     StartActivity(new Intent(this, typeof(loginactivity)));
                 })
                 .SetNegativeButton("取消", (s1, e1) =>
                 {
                     return;
                 });
                alertDialog.Create().Show();
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetCancelable(true);
                string[] btns = new string[] { "删除", "修改" };
                //var model = answerCommentList.Find(f => f.AnswerId ==ans);
                builder.SetItems(btns, (s, e) =>
                {
                    if (e.Which == 0)
                    {
                        Delete(questionId, answerId, int.Parse(tag));
                    }
                    else if (e.Which == 1)
                    {
                        QuestionEditAnswerCommentActivity.Enter(this, questionId, answerId, int.Parse(tag), answerCommentList[position].Content);
                    }
                }).Show();
            }
         }
        void Delete(int questionId, int answerId,int commentId)
        {
                    ProgressDialog progressDialog = new ProgressDialog(this);
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressDialog.SetMessage("删除中....");
                    progressDialog.Show();
                    QuestionRequest.DeleteQuestionAnswerComment(userToken, questionId, answerId, commentId, (error) =>
                    {
                        RunOnUiThread(() =>
                        {
                            progressDialog.Hide();
                            AlertUtil.ToastShort(this, "删除评论失败" + error);
                        });
                    }, () =>
                    {
                        RunOnUiThread(() =>
                        {
                            progressDialog.Hide();
                            AlertUtil.ToastShort(this, "删除评论成功");
                            OnRefresh();
                        });
                    });
           }

        public async void OnRefresh()
        {
            var result = await  QuestionRequest.ListQuestionAnswerComment(userToken, answerId);
            if (result.Success)
            {
                answerCommentList = result.Data;
                adapter.SetNewData(answerCommentList);
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
            }
            _swipeRefreshLayout.PostDelayed(() =>
            {
                _swipeRefreshLayout.Refreshing = false;
            }, 1000);
        }

    }
}
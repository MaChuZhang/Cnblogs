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
    public  class QuestionAnswerActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
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
        private BaseRecyclerViewAdapter<QuestionAnswerViewModel> adapter;
        public string   blogApp; //博客名
        private DisplayImageOptions options;
        private int questionId;
        private EditText edit_content;
        private Button btn_submit;
        private bool isAt;
        private string atUserName;
        private List<QuestionAnswerViewModel> answerList = new List<QuestionAnswerViewModel>();
        private Token userToken;
        private UserInfo userInfo;
        internal static void Enter(Context context,int questionId)
        {
            Intent intent = new Intent(context, typeof(QuestionAnswerActivity));
            intent.PutExtra("questionId", questionId);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            questionId = Intent.GetIntExtra("questionId", 0);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.comment));
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
            var result = await QuestionService.ListQuestionAnswer(AccessTokenUtil.GetToken(this), questionId);
            if (result.Success)
            {
                answerList = result.Data;
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
                var result = await QuestionService.AddQuestionAnswer(userToken, userInfo.BlogApp, userInfo.DisplayName, body, questionId, userInfo.SpaceUserId);
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
            var result = await QuestionService.ListQuestionAnswer(userToken, questionId);
            if (result.Success)
            {
                var tempList = result.Data;
                answerList.AddRange(tempList);
                adapter.SetNewData(answerList);
            }
            else
            {
                AlertUtil.ToastShort(this,result.Message);
            }
        }
         void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<QuestionAnswerViewModel>(this, answerList, Resource.Layout.item_recyclerview_questionAnswer, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                QuestionAnswerCommentActivity.Enter(this,questionId,int.Parse(tag));
            };
            adapter.ItemLongClick += ItemLongClick;
            string read = Resources.GetString(Resource.String.read);
            string comment = Resources.GetString(Resource.String.comment);
            string digg = Resources.GetString(Resource.String.digg);

            adapter.OnConvertView += (holder, position) =>
            {
                var model = answerList[position];
                holder.SetText(Resource.Id.tv_commentUserName,model.UserName);
                holder.SetText(Resource.Id.tv_commentDateAdded, model.DateAdded.ToCommonString());
                if (!string.IsNullOrEmpty(model.Answer.Trim()))
                {
                    (holder.GetView<TextView>(Resource.Id.tv_commentContent)).SetText(HtmlUtil.GetHtml(model.Answer.Trim()), TextView.BufferType.Spannable);
                }
                //holder.SetText(Resource.Id.tv_userName, model.UserName);
                holder.GetView<LinearLayout>(Resource.Id.ly_item).Tag = model.AnswerID.ToString();
                holder.SetText(Resource.Id.tv_commentCount, model.CommentCounts.ToString());
                holder.SetText(Resource.Id.tv_scoreName, HtmlUtil.GetScoreName(model.Score));
                if (model.IsBest)
                {
                    holder.GetView<TextView>(Resource.Id.tv_isBest).Visibility = ViewStates.Visible;
                }
                else
                {
                    holder.GetView<TextView>(Resource.Id.tv_isBest).Visibility = ViewStates.Gone;
                }
                if (model != null)
                {
                    holder.SetImageLoader(Resource.Id.iv_commentUserIcon, options, Constact.CnblogsPic +model.AnswerUserInfo.IconName);
            
                }
            };
        }

        void ItemLongClick(string  tag ,int position)
        {
            if (userInfo.DisplayName == answerList[position].UserName)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetCancelable(true);
                string[] btns = new string[] { "删除", "修改" };
                int answerId = int.Parse(tag);
                builder.SetItems(btns, (s, e) =>
                {
                    if (e.Which == 0)
                    {
                        ProgressDialog progressDialog = new ProgressDialog(this);
                        progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                        progressDialog.SetMessage("删除中....");
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
                        progressDialog.Show();
                        QuestionService.DeleteQuestionAnswer(userToken, questionId, answerId, (error) =>
                        {
                            RunOnUiThread(() =>
                            {
                                progressDialog.Hide();
                                AlertUtil.ToastShort(this, "删除回答失败" + error);
                            });
                        }, () =>
                        {
                            RunOnUiThread(() =>
                            {
                                progressDialog.Hide();
                                AlertUtil.ToastShort(this, "删除回答成功");
                            });
                        });
                    }
                    else if(e.Which==1)
                    {
                        QuestionEditAnswerActivity.Enter(this,questionId,answerId, answerList[position].Answer);
                    }
                }).Show();
            }
        }
        void Delete(int questionId, int answerId)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetCancelable(true);
            string[] btns = new string[] { "删除","修改"};
            //var model = answerList.Find(f => f.AnswerId ==ans);
            builder.SetItems(btns, (s, e) =>
            {
                if (e.Which == 0)
                {
                    ProgressDialog progressDialog = new ProgressDialog(this);
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressDialog.SetMessage("删除中....");
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
                    progressDialog.Show();
                    QuestionService.DeleteQuestionAnswer(userToken, questionId, answerId, (error) =>
                    {
                        RunOnUiThread(() =>
                        {
                            progressDialog.Hide();
                            AlertUtil.ToastShort(this, "删除回答失败" + error);
                        });
                    }, () =>
                    {
                        RunOnUiThread(() =>
                        {
                            progressDialog.Hide();
                            AlertUtil.ToastShort(this, "删除回答成功");
                        });
                    });
                }
                else if (e.Which==1)
                {
                    QuestionEditAnswerActivity.Enter(this,questionId,answerId,answerList.Find(f=>f.AnswerID==answerId).Answer);
                }
            }).Show();
           
        }

        public async void OnRefresh()
        {
            var result = await  QuestionService.ListQuestionAnswer(userToken, questionId);
            if (result.Success)
            {
                answerList = result.Data;
                adapter.SetNewData(answerList);
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
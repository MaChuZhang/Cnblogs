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
using Android.Support.V4.App;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/comment",Theme ="@style/AppTheme")]
    public  class QuestionEditAnswerActivity : BaseActivity
    {
        
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.editQuestionAnswerComment;
            }
        }
        protected override string ToolBarTitle
        {
            get
            {
                return "编辑回答";
            }
        }
      
        private int answerId,questionId;
        private string content;
        private EditText edit_content;
        private Button btn_submit;
        private Token userToken;
        private UserInfo userInfo;
        internal static void Enter(Context context,int questionId, int answerId,string content)
        {
            Intent intent = new Intent(context, typeof(QuestionEditAnswerActivity));
            intent.PutExtra("answerId", answerId);
            intent.PutExtra("questionId", questionId);
            intent.PutExtra("content", content);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            answerId = Intent.GetIntExtra("answerId", 0);
            questionId = Intent.GetIntExtra("questionId", 0);
            content = Intent.GetStringExtra("content");
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle("回答评论");
            //显示图片配置
            edit_content = FindViewById<EditText>(Resource.Id.edit_content);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            userToken = UserTokenUtil.GetToken(this);
            if (!string.IsNullOrEmpty(content))
            {
                edit_content.SetText(HtmlUtil.GetHtml(content), TextView.BufferType.Spannable);
                btn_submit.Enabled = true;
            }
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
                 
                }
                else btn_submit.Enabled = false;
            };
            userInfo = UserInfoShared.GetUserInfo(this);
        }
        private  void Add()
        {
            string body = edit_content.Text.TrimEnd().TrimStart();
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
               QuestionRequest.EditQuestionAnswer(userToken, body, questionId, answerId,userInfo.SpaceUserId,()=> {
                   RunOnUiThread(() =>
                   {
                       dialog.Hide();
                       AlertUtil.ToastShort(this, "评论成功");
                       edit_content.Text = "";
                       btn_submit.Enabled = false;
                       ActivityCompat.FinishAfterTransition(this);
                   });
                },(error)=> {
                    RunOnUiThread(() =>
                    {
                        dialog.Hide();
                        AlertUtil.ToastShort(this, error);
                        btn_submit.Enabled = true;
                    });
                });
            }
        }


    }
}
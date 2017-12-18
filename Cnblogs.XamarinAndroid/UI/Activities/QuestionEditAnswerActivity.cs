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
                return "�༭�ش�";
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
            SetToolBarTitle("�ش�����");
            //��ʾͼƬ����
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
                .SetTitle("��¼��ʾ")
                 .SetMessage("δ��¼���¼token�Ѿ�����")
                 .SetPositiveButton("��Ȩ", (s1, e1) =>
                 {
                     StartActivity(new Intent(this, typeof(loginactivity)));
                 })
                 .SetNegativeButton("ȡ��", (s1, e1) =>
                 {
                     return;
                 });
                alertDialog.Create().Show();
            }
            else
            {
                if (string.IsNullOrEmpty(body))
                {
                    AlertUtil.ToastShort(this, "����������");
                    return;
                }
                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetTitle("����");
                dialog.SetMessage("�ύ������.....");
                dialog.Show();
               QuestionRequest.EditQuestionAnswer(userToken, body, questionId, answerId,userInfo.SpaceUserId,()=> {
                   RunOnUiThread(() =>
                   {
                       dialog.Hide();
                       AlertUtil.ToastShort(this, "���۳ɹ�");
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
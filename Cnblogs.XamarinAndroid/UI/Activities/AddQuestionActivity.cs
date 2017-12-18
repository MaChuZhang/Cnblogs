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
using Cnblogs.HttpClient;
using System.Threading;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "AddQuestionActivity", Theme = "@style/AppTheme")]
    public class AddQuestionActivity : BaseActivity
    {
        protected override int LayoutResourceId => Resource.Layout.addQuestion;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.question_add);
        private EditText edit_content, edit_title, edit_tags;
        private Button btn_submit;
        private TextView tv_startLogin;
        private LinearLayout ly_expire, ll_content;
        private string flags="1";//是否发送至首页
        private int wzLinkId;
        private RadioGroup rg_flags;
        private RadioButton rb_true, rb_false;
        private Token userToken;
        internal static void Enter(Context context)
        {
            Intent intent = new Intent(context, typeof(AddQuestionActivity));
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarNavBack();

            rg_flags = FindViewById<RadioGroup>(Resource.Id.rg_flags);
            rb_false = FindViewById<RadioButton>(Resource.Id.rb_false);
            rb_true = FindViewById<RadioButton>(Resource.Id.rb_true);

            edit_title = FindViewById<EditText>(Resource.Id.edit_title);
            edit_tags = FindViewById<EditText>(Resource.Id.edit_tags);
            edit_content = FindViewById<EditText>(Resource.Id.edit_content);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            btn_submit.Enabled = true;
            ly_expire = FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = FindViewById<TextView>(Resource.Id.tv_startLogin);
            ll_content = FindViewById<LinearLayout>(Resource.Id.ll_content);
            
            ll_content.Click += (s, e) =>
            {
                edit_content.Focusable = true;
            };
            tv_startLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(this, typeof(loginactivity)));
            };
            userToken = UserTokenUtil.GetToken(this);
            //radiobutton单击事件
            rg_flags.CheckedChange += (s, e) =>
            {
                if (e.CheckedId == rb_true.Id)
                {
                    flags = "1";
                }
                else if (e.CheckedId == rb_false.Id)
                {
                    flags = "0";
                }
            };
            //验证是否登录
            if (userToken.IsExpire)
            {
                btn_submit.Enabled = false;
                ly_expire.Visibility = ViewStates.Visible;
            }
            else
            {
                btn_submit.Enabled = true;
                ly_expire.Visibility = ViewStates.Gone;
            }
            //验证提交按钮
            edit_title.TextChanged += (s, e) =>
            {
                if (string.IsNullOrEmpty(edit_title.Text.Trim())|| string.IsNullOrEmpty(edit_content.Text.Trim()))
                    btn_submit.Enabled = false;
                else
                    btn_submit.Enabled = true;
            };
            edit_content.TextChanged += (s, e) =>
            {
                if (string.IsNullOrEmpty(edit_content.Text.Trim())|| string.IsNullOrEmpty(edit_title.Text.Trim()))
                    btn_submit.Enabled = false;
                else
                    btn_submit.Enabled = true;
            };
            btn_submit.Click += AddQuestionClick;
        }

        public async void AddQuestionClick(object o, EventArgs e)
        {
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
            string content = edit_content.Text.Trim();
            string title = edit_content.Text.Trim();
            string tags = edit_tags.Text.Trim();
            if (string.IsNullOrEmpty(content) || content.Length > 100000 || content.Length < 20)
            {
                AlertUtil.ToastShort(this, "提问内容20-100000个字符之间");
                return;
            }
            if (string.IsNullOrEmpty(title) || title.Length > 100000 || title.Length < 20)
            {
                AlertUtil.ToastShort(this, "标题6-200分字符");
                return;
            }
            if (!string.IsNullOrEmpty(tags))
            {
                var tagCount = tags.Split(',').Length;
                if (tagCount > 5)
                {
                    AlertUtil.ToastShort(this, "不能超过5个标签,以中文都好隔开");
                    return;
                }
            }
            var userInfo = UserInfoShared.GetUserInfo(this);
            var dialog = new ProgressDialog(this);
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.SetCancelable(false);
            dialog.SetCanceledOnTouchOutside(false);
            dialog.SetTitle(Resources.GetString(Resource.String.bookmark_add));
            dialog.SetMessage("提交中.......");
            dialog.Show();
            var result = await QuestionRequest.Add(userToken, title, content, tags, flags, userInfo.SpaceUserId.ToString());
            if (result.Success)
            {
                RunOnUiThread(() =>
                {
                    dialog.Hide();
                    edit_content.Text = "";
                    edit_title.Text = "";
                    edit_tags.Text = "";
                    rb_true.Checked = true;
                    AlertUtil.ToastShort(this,"发布问答成功");
                    ActivityCompat.FinishAfterTransition(this);
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    dialog.Hide();
                    AlertUtil.ToastShort(this,result.Message);
                });
            }
        }
        public override bool OnMenuItemClick(IMenuItem item)
        {
            //if (article != null)
            //{
            //    // sharesWidget
            //    shareWidget.Open(article.Url,article.Title);
            //}
            return true;
        }

        //public void OnClick(View v)
        //{
        //    ActivityCompat.FinishAfterTransition(this);
        //}
    }
}
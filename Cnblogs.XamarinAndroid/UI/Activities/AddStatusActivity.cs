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
    [Activity(Label = "AddStatusActivity", Theme = "@style/AppTheme")]
    public class AddStatusActivity : BaseActivity
    {
        protected override int LayoutResourceId => Resource.Layout.addStatus;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.status_add);
        private Button btn_submit;
        private TextView tv_startLogin;
        private LinearLayout ly_expire;
        private RadioGroup rg_isPrivate;
        private RadioButton rb_false,rb_true;
        private EditText et_content;
        internal static void Enter(Context context)
        {
            Intent intent = new Intent(context, typeof(AddStatusActivity));
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarNavBack();

            rg_isPrivate = FindViewById<RadioGroup>(Resource.Id.rg_isPrivate);
            rb_false = FindViewById<RadioButton>(Resource.Id.rb_false);
            rb_true = FindViewById<RadioButton>(Resource.Id.rb_true);
            btn_submit = FindViewById<Button>(Resource.Id.btn_submit);
            btn_submit.Enabled = false;
            ly_expire = FindViewById<LinearLayout>(Resource.Id.ly_expire);
            tv_startLogin = FindViewById<TextView>(Resource.Id.tv_startLogin);
            et_content = FindViewById<EditText>(Resource.Id.et_content);
            tv_startLogin.Click += (s, e) =>
            {
                StartActivity(new Intent(this,typeof(loginactivity)));
            };

            AlertUtil.ToastLong(this, "当前线程id:" + Thread.CurrentThread.ManagedThreadId);

            string  title = Intent.GetStringExtra("title");
            string url = Intent.GetStringExtra("url");
            

       
            Token token = UserTokenUtil.GetToken(this);
      
            if (token.IsExpire)
            {
                btn_submit.Enabled = false;
                ly_expire.Visibility = ViewStates.Visible;
            }
            else
            {
                btn_submit.Enabled = true;
                ly_expire.Visibility = ViewStates.Gone;
            }
            et_content.TextChanged += (s, e) =>
            {
                string content = et_content.Text.Trim();
                if (!string.IsNullOrEmpty(content))
                {
                    btn_submit.Enabled = true;
                }
                else
                    btn_submit.Enabled = false;
            };
            btn_submit.Click += SubmitClick;
        }
        public async void SubmitClick(object o, EventArgs eventargs)
        {
            bool isPrivate = false;
            var userToken = UserTokenUtil.GetToken(this);
            if (et_content.Text.Trim().Length == 0)
                return;
            rg_isPrivate.CheckedChange += (s, e) =>
            {
                if (e.CheckedId == rb_true.Id)
                {
                    isPrivate = true;
                }
                else if (e.CheckedId == rb_false.Id)
                {
                    isPrivate = false;
                }
            };
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
            var dialog = new ProgressDialog(this);
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.SetCancelable(false);
            dialog.SetCanceledOnTouchOutside(false);
            dialog.SetTitle(Resources.GetString(Resource.String.bookmark_add));
            dialog.SetMessage("提交中.......");
            dialog.Show();
            var result = await StatusRequest.Add(userToken, et_content.Text.Trim(), isPrivate);
            if (result.Success)
            {
                dialog.Hide();
                AlertUtil.ToastShort(this, "发布成功");
                ActivityCompat.FinishAfterTransition(this);
            }
            else
            {
                dialog.Hide();
                AlertUtil.ToastShort(this,result.Message);
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
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
using Com.Umeng.Analytics;

namespace Cnblogs.XamarinAndroid.UI.Activities
{
    [Activity(Label = "",Theme ="@style/AppTheme")]
    public class SettingActivity : BaseActivity
    {
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.Setting;
            }
        }
        protected  override  string ToolBarTitle
        {
            get
            {
                return  Resources.GetString(Resource.String.setting);
            }
        } private Button btn_exitLogin;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            btn_exitLogin = FindViewById<Button>(Resource.Id.btn_exitLogin);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.setting));
            UpdateViewStatus();
            btn_exitLogin.Click += (s, e) =>
            {
                Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle("提示")
                 .SetMessage("你确定退出当前账号?")
                 .SetPositiveButton("确定", (s1, e1) =>
                 {
                     //UserBlogShared.SetUserBlog();
                     UserUtil.Instance(this).Logout();
                     MobclickAgent.OnProfileSignOff();
                     UpdateViewStatus();
                    //AlertUtil.ToastLong(this,"成功");
                 })
                 .SetNegativeButton("取消", (s1, e1) =>
                 {
                     return;
                 });
                alertDialog.Create().Show();
            };
        }
        void UpdateViewStatus()
        {
            if (UserUtil.Instance(this).LoginExpire())
            {
                btn_exitLogin.Enabled = false;
            }
            else
            {
                btn_exitLogin.Enabled = true;
            }
        }
        internal static void Enter(Context context)
        {
            Intent intent = new Intent(context,typeof(SettingActivity));
            context.StartActivity(intent);
        }
    }
}
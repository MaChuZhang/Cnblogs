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
using Com.Iflytek.Autoupdate;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "AboutActivity")]
    public class AboutActivity : BaseActivity, IFlytekUpdateListener
    {
        private IFlytekUpdate updManager;
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.About;
            }
        }
        protected override string ToolBarTitle
        {
            get
            {
                return "关于";
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarUtil.SetColorStatusBars(this);
            LinearLayout ll_csdn = FindViewById<LinearLayout>(Resource.Id.ll_csdn);
            LinearLayout ll_qq = FindViewById<LinearLayout>(Resource.Id.ll_qq);
            LinearLayout ll_joinQQGroup = FindViewById<LinearLayout>(Resource.Id.ll_joinQQGroup);
            LinearLayout ll_version = FindViewById<LinearLayout>(Resource.Id.ll_version);
            updManager = IFlytekUpdate.GetInstance(this.ApplicationContext);
            updManager.SetDebugMode(true);
            updManager.SetParameter(UpdateConstants.ExtraWifionly, "true");
            updManager.SetParameter(UpdateConstants.ExtraNotiIcon, "true");
            ll_version.Click += (s, e) =>
            {
                updManager.SetParameter(UpdateConstants.ExtraStyle, UpdateConstants.UpdateUiDialog);
                updManager.AutoUpdate(this, this);
            };
            ll_csdn.Click += (s, e) =>
            {
                //  Intent intent = new Intent();
                //intent.SetAction("android.intent.action.VIEW");
                Android.Net.Uri uri =Android.Net.Uri.Parse("http://blog.csdn.net/kebi007");
                Intent intent = new Intent();
                intent.SetAction("android.intent.action.VIEW");
                intent.SetData(uri);
                StartActivity(intent);
            };
            ll_joinQQGroup.Click += (s, e) =>
            {
                joinQQGroup("Q90lFEYkStricLdyCyJQMzJJq_5bo7gb");
            };
            ll_qq.Click += (s, e) =>
            {
                string qqUrl = "mqqwpa://im/chat?chat_type=wpa&uin=976720945&version=1";
                StartActivity(new Intent(Intent.ActionView,Android.Net.Uri.Parse(qqUrl)));
            };
            // Create your application here
        }
        internal static void Enter(Context context)
        {
            context.StartActivity(new  Intent(context,typeof(AboutActivity)));
        }

        /****************
*
* 发起添加群流程。群号：xamarin android,C#,Web前端(371026625) 的 key 为： Q90lFEYkStricLdyCyJQMzJJq_5bo7gb
* 调用 joinQQGroup(Q90lFEYkStricLdyCyJQMzJJq_5bo7gb) 即可发起手Q客户端申请加群 xamarin android,C#,Web前端(371026625)
*
* @param key 由官网生成的key
* @return 返回true表示呼起手Q成功，返回fals表示呼起失败
******************/
        public bool joinQQGroup(string key)
        {
            Intent intent = new Intent();
            intent.SetData(Android.Net.Uri.Parse("mqqopensdkapi://bizAgent/qm/qr?url=http%3A%2F%2Fqm.qq.com%2Fcgi-bin%2Fqm%2Fqr%3Ffrom%3Dapp%26p%3Dandroid%26k%3D" + key));
            // 此Flag可根据具体产品需要自定义，如设置，则在加群界面按返回，返回手Q主界面，不设置，按返回会返回到呼起产品界面    //intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            try
            {
                StartActivity(intent);
                return true;
            }
            catch (Exception e)
            {
                // 未安装手Q或安装的版本不支持
                return false;
            }
        }

        public void OnResult(int errorCode, UpdateInfo result)
        {
            if (errorCode == UpdateErrorCode.Ok && result != null)
            {
                if (result.UpdateType == UpdateType.NoNeed)
                {
                    Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle("检查更新");
                    alertDialog.SetMessage("当前版本" + Resources.GetString(Resource.String.version) + "已经是最新的");
                    alertDialog.Show();
                }
                else
                updManager.ShowUpdateInfo(this, result);
            }
        }
    }
}
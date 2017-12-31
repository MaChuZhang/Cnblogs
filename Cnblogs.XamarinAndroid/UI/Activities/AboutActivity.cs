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
                return "����";
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
* �������Ⱥ���̡�Ⱥ�ţ�xamarin android,C#,Webǰ��(371026625) �� key Ϊ�� Q90lFEYkStricLdyCyJQMzJJq_5bo7gb
* ���� joinQQGroup(Q90lFEYkStricLdyCyJQMzJJq_5bo7gb) ���ɷ�����Q�ͻ��������Ⱥ xamarin android,C#,Webǰ��(371026625)
*
* @param key �ɹ������ɵ�key
* @return ����true��ʾ������Q�ɹ�������fals��ʾ����ʧ��
******************/
        public bool joinQQGroup(string key)
        {
            Intent intent = new Intent();
            intent.SetData(Android.Net.Uri.Parse("mqqopensdkapi://bizAgent/qm/qr?url=http%3A%2F%2Fqm.qq.com%2Fcgi-bin%2Fqm%2Fqr%3Ffrom%3Dapp%26p%3Dandroid%26k%3D" + key));
            // ��Flag�ɸ��ݾ����Ʒ��Ҫ�Զ��壬�����ã����ڼ�Ⱥ���水���أ�������Q�����棬�����ã������ػ᷵�ص������Ʒ����    //intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            try
            {
                StartActivity(intent);
                return true;
            }
            catch (Exception e)
            {
                // δ��װ��Q��װ�İ汾��֧��
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
                    alertDialog.SetTitle("������");
                    alertDialog.SetMessage("��ǰ�汾" + Resources.GetString(Resource.String.version) + "�Ѿ������µ�");
                    alertDialog.Show();
                }
                else
                updManager.ShowUpdateInfo(this, result);
            }
        }
    }
}
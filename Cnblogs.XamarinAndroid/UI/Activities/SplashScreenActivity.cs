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
using Cnblogs.HttpClient;
using Android.Support.V7.App;

namespace Cnblogs.XamarinAndroid.UI.Activities
{
    [Activity(Label = "@string/ApplicationName", Theme = "@style/SplashScreen", MainLauncher = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarUtil.SetColorStatusBars(this);
            SetContentView(Resource.Layout.SplashScreen);
            ClipboardManager clipboard = (ClipboardManager)GetSystemService(Context.ClipboardService);
            clipboard.Text = "复制此消息，打开最新版支付宝就能领取！ELtcyi16Ax";
            var tokenTemp = AccessTokenUtil.GetToken(this);
            if (string.IsNullOrEmpty(tokenTemp.access_token)||tokenTemp.IsExpire)
            {
                await AuthorizationRequest.Client_Credentials((token) =>
                {
                    token.RefreshTime = DateTime.Now;
                    AccessTokenUtil.SaveToken(token, this);
                }, error =>
                {
                    System.Diagnostics.Debug.Write(error);
                });

            }
            Handler handler = new Handler();
            handler.PostDelayed((() =>
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                this.Finish();
            }), 2000);
            // Create your application here
        }
    }
}
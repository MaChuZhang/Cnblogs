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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Webkit;
using Cnblogs.HttpClient;
using Cnblogs.ApiModel;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using Cnblogs.XamarinAndroid;
using Android.Support.V4.App;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "loginactivity", Theme = "@style/AppTheme",MainLauncher  =false)]
    public class loginactivity:BaseActivity
    {
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.Login;
            }
        }
        private Toolbar toolbar;
        private WebView loginview;
        private ProgressBar progressBar;
        //private static Context context => this;
        protected override async void OnCreate(Bundle savedinstancestate)
        {
            base.OnCreate(savedinstancestate);
            StatusBarUtil.SetColorStatusBars(this);
            // create your application here
            // toolbar = findviewbyid<toolbar>(resource.id.toolbar);
           // toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
           //toolbar.Title = "µÇÂ¼";
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar_login);

         //   SetSupportActionBar(toolbar);
           // SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //toolbar.setnavigationonclicklistener(this);
            //toolbar.NavigationClick += (s, e) =>
            //{
            //    SetResult(Result.Canceled);
            //    this.Finish();
            //};
            progressBar.Max = 100;
            loginview = FindViewById<WebView>(Resource.Id.webview_login);
            loginview.Settings.SetSupportZoom(true);
            loginview.Settings.JavaScriptEnabled = true;
            loginview.Settings.BuiltInZoomControls = true;
            loginview.Settings.CacheMode = CacheModes.NoCache;
            WebViewClient webClient = new WebViewClient();
            loginview.SetWebChromeClient(new loginwebchromeclient(progressBar));
            loginview.SetWebViewClient(new loginwebviewclient(this));
            string url = string.Format(Constact.GetAuthrize, Constact.client_id);
            loginview.LoadUrl(url);
        }
    }

    public class loginwebchromeclient : WebChromeClient
    {
        private ProgressBar progressbar;
        public loginwebchromeclient(ProgressBar _progress)
        {
            progressbar = _progress;
        }
        public override void OnProgressChanged(WebView view, int newprogress)
        {
            progressbar.Progress = newprogress;
            if (newprogress < 100)
            {
                if (progressbar.Visibility == ViewStates.Gone) progressbar.Visibility = ViewStates.Visible;
            }
            else progressbar.Visibility = ViewStates.Gone;
            base.OnProgressChanged(view, newprogress);
        }
    };

    public class loginwebviewclient : WebViewClient
    {
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return base.ShouldInterceptRequest(view, request);

        }
        private Context context;
       // private static Token token = AccessTokenUtil.GetToken(context);
        public loginwebviewclient(Context _context)
        {
            context = _context;
        }
        [Obsolete]
        public override  bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url.IndexOf(Constact.Callback) > -1)
            {
                Uri uri = new Uri(url.Replace("#", "?"));
                var query = uri.Query.TrimStart('?').Split('&');
                foreach (var item in query)
                {
                    var q = item.Split('=');
                    if (q[0] == "code")
                    {
                        var code = q[1];
                        System.Diagnostics.Debug.Write(code);
                        Token token = AccessTokenUtil.GetToken(context);
                        AuthorizationRequest.Authorization_Code(token, code, (userToken) =>
                         {
                             System.Diagnostics.Debug.Write(userToken.access_token);
                             UserTokenUtil.SaveToken(userToken, context);
                             // ActivityCompat.FinishAfterTransition(context);
                             context.StartActivity(new Intent(context, typeof(MainActivity)));
                         },
                        error =>
                        {
                            System.Diagnostics.Debug.Write(error);
                        });
                    }
                }
                // view.stoploading();
            }
            return base.ShouldOverrideUrlLoading(view, url);
        }
    }
    //public class LoginToken {
    //    public void OnLogin(string code)
    //    {
    //       // dialog.Show();
    //        var cientId = Constact.client_secret_firend;
    //        var clientSercret = Constact.client_secret;
    //        var grant_type = "authorization_code";
    //        var redirect_uri = "https://oauth.cnblogs.com/auth/callback";

    //        var content = string.Format("client_id={0}&client_secret={1}&grant_type={2}&redirect_uri={3}&code={4}", cientId, clientSercret, grant_type, redirect_uri, code);
    //       // loginPresenter.Login(TokenShared.GetAccessToken(this), content);
    //    }
    //}

}
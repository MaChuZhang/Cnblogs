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
using Cnblogs.XamarinAndroid.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Webkit;
using Cnblogs.HttpClient;
using Cnblogs.ApiModel;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using Cnblogs.XamarinAndroid;

namespace Cnblogs.XamarinAndroid.UI.Activities
{
    [Activity(Label = "LoginActivity",Theme = "@style/BaseAppTheme")]
    public class LoginActivity : BaseActivity
    {
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.Login;
            }
        }
        private Toolbar toolbar;
        private WebView loginView;
        private ProgressBar progressBar;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarUtil.SetColorStatusBars(this);
            // Create your application here
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            toolbar.Title = "µÇÂ¼";
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar_login);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //toolbar.SetNavigationOnClickListener(this);
            toolbar.NavigationClick += (s, e) =>
            {
                SetResult(Result.Canceled);
                this.Finish();
            };
            progressBar.Max = 100;
            loginView = FindViewById<WebView>(Resource.Id.webview_login);
            loginView.Settings.SetSupportZoom(true);
            loginView.Settings.JavaScriptEnabled = true;
            loginView.Settings.BuiltInZoomControls = true;
            loginView.Settings.CacheMode = CacheModes.NoCache;
            WebChromeClient webClient = new WebChromeClient();
            loginView.SetWebChromeClient(new LoginWebChromeClient(progressBar));
            loginView.SetWebViewClient(new LoginWebViewClient());
            //await UserRequest.Login("123", (token) =>
            //{
            //    SharedDataUtil.SaveToken(token,this);
            //    //System.Diagnostics.Debug.Write(token.access_token);
            //    //Toast.MakeText(Activity,ModelFactory.token.access_token,ToastLength.Short).Show();
            //}, error =>
            //{
            //    System.Diagnostics.Debug.Write(error);
            //});
            //await ArticleRequest.GetArticleList((list) =>
            //{
            //    System.Diagnostics.Debug.Write(list[0].BlogApp);
            //}, (error) =>
            //{
            //    System.Diagnostics.Debug.Write(error);
            //});

        }
    }

    public class LoginWebChromeClient : WebChromeClient {
        private ProgressBar progressBar;
        public LoginWebChromeClient(ProgressBar _progress)
        {    
            progressBar = _progress;
        }
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            progressBar.Progress = newProgress;
            if (newProgress < 100)
            {
                if (progressBar.Visibility == ViewStates.Gone) progressBar.Visibility = ViewStates.Visible;
            }
            else progressBar.Visibility = ViewStates.Gone;
            base.OnProgressChanged(view, newProgress);
        }
    };

    public class LoginWebViewClient : WebViewClient
    {
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return base.ShouldInterceptRequest(view, request);

        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url.IndexOf(Constact.Callback)>-1)
            {
                Uri uri = new Uri(url.Replace("#","?"));
                var query = uri.Query.TrimStart('?').Split('&');
                foreach (var  item  in query)
                {
                    var q = item.Split('=');
                    if (q[0] == "code")
                    {
                        var code = q[1];
                        System.Diagnostics.Debug.Write(code);
                        //UserRequest.Login(code, (token) =>
                        //{
                        //    System.Diagnostics.Debug.Write(ModelFactory.token);
                        //    //Toast.MakeText(Activity,ModelFactory.token.access_token,ToastLength.Short).Show();
                        //}, error =>
                        //{
                        //    System.Diagnostics.Debug.Write(error);
                        //});
                    }
                }
               // view.StopLoading();
            }
            return base.ShouldOverrideUrlLoading(view, url);
        }
    }
    public class LoginToken {
        public void OnLogin(string code)
        {
           // dialog.Show();
            var cientId = Constact.client_secret_firend;
            var clientSercret = Constact.client_secret;
            var grant_type = "authorization_code";
            var redirect_uri = "https://oauth.cnblogs.com/auth/callback";

            var content = string.Format("client_id={0}&client_secret={1}&grant_type={2}&redirect_uri={3}&code={4}", cientId, clientSercret, grant_type, redirect_uri, code);
           // loginPresenter.Login(TokenShared.GetAccessToken(this), content);
        }
    }

}
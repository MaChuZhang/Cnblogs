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

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "DetailArticleActivity",Theme = "@style/AppTheme")]
    public class DetailKbArticlesActivity : BaseActivity
    {
        //private Toolbar toolbar;
        private TextView tv_view;
        private WebView wb_content;
        private int _id;
        private Button btn_digg;
        private Button btn_mark;
        private KbArticles kbArticles;
        private UMengShareWidget shareWidget;
        
        protected override int LayoutResourceId => Resource.Layout.detailKbArticles;

        protected override string ToolBarTitle => string.Empty;
        internal static void Enter(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(DetailKbArticlesActivity));
            intent.PutExtra("id",id);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);

            SetToolBarNavBack();
            wb_content = FindViewById<WebView>(Resource.Id.wb_content);
            btn_digg = FindViewById<Button>(Resource.Id.btn_digg);
            btn_mark = FindViewById<Button>(Resource.Id.btn_mark);
            tv_view = FindViewById<TextView>(Resource.Id.tv_view);

            btn_mark.Click += (s, e) =>
            {
                AddBookmarkActivity.Enter(this,string.Format(Constact.KbPage, _id), kbArticles.Title, "add");
            };

            wb_content.Settings.DomStorageEnabled = true;
            wb_content.Settings.JavaScriptEnabled = true;//支持js
            wb_content.Settings.DefaultTextEncodingName = "utf-8";//设置编码方式utf-8
            wb_content.Settings.SetSupportZoom(false);//不可缩放
            wb_content.Settings.DisplayZoomControls = false;//隐藏原生的缩放控件
            wb_content.Settings.BuiltInZoomControls = false;//设置内置的缩放控件
            wb_content.Settings.CacheMode = CacheModes.CacheElseNetwork;
            wb_content.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            wb_content.Settings.LoadsImagesAutomatically = true;//支持自动加载图片
            wb_content.Settings.UseWideViewPort = true; //将图片调整到合适webview的大小
            wb_content.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            var jsInterface = new  WebViewJSInterface(this);
            wb_content.SetWebViewClient(ContentWebViewClient.Instance(this));
            wb_content.AddJavascriptInterface(jsInterface,"openlistner");
            jsInterface.CallFromPageReceived += delegate (object sender,WebViewJSInterface.CallFromPageReceivedEventArgs e)
            {
                  PhotoActivity.Enter(this,e.Result.Split(','),e.Index);
            };
            _id = Intent.GetIntExtra("id",0);
            GetClientArticle(_id);
            //shareWidget = new UMengShareWidget(this);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share,menu);
            return  true;
        }

        async void GetClientArticle(int id)
        {
             kbArticles = await SQLiteUtil.SelectKbArticles(id);
            if (kbArticles != null)
            {
                 tv_view.Text = kbArticles.ViewCount.ToString();
                 btn_digg.Text = kbArticles.Diggcount.ToString();
                 SetToolBarTitle(kbArticles.Title);
                 if (string.IsNullOrEmpty(kbArticles.Content))
                    GetRequestArticle(id);
                 else
                 {
                    kbArticles.Content = kbArticles.Content.ReplaceHtml().Trim('"');
                    string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", kbArticles.Content).Replace("#title#", kbArticles.Title).Replace("#author#","作者："+kbArticles.Author).Replace("#date#","发布日期："+kbArticles.DateAdded.ToString("yyyy年MM月dd日 HH:mm"));
                    wb_content.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                }
            }
        }
        void GetAssetContent(string body)
        {
            string content = HtmlUtil.ReadHtml(Assets);
            body = body.ReplaceHtml();

        }

        async void GetRequestArticle(int id)
        {
            try
            {
                var result = await HttpClient.KbArticlesRequest.GetKbArticlesDetail(AccessTokenUtil.GetToken(this),id);
                if (result.Success)
                {
                    await SQLiteUtil.SelectKbArticles(id).ContinueWith(async (r) =>
                    {
                        kbArticles = r.Result;
                        kbArticles.Content = result.Data;
                        await SQLiteUtil.UpdateKbArticles(kbArticles);
                        kbArticles.Content = kbArticles.Content.ReplaceHtml().Trim('"');
                        string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", kbArticles.Content).Replace("#title#",kbArticles.Title).Replace("#author#","作者："+kbArticles.Author).Replace("#date#","发布日期："+kbArticles.DateAdded.ToString("yyyy年MM月dd日 HH:mm"));
                        wb_content.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (kbArticles != null)
            {
                // sharesWidget
                //shareWidget.Open(kbArticles.Url,kbArticles.Title);
            }
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode,resultCode,data);
            UMShareAPI.Get(this).OnActivityResult(requestCode,(int)resultCode,data);
        }

        //public void OnClick(View v)
        //{
        //    ActivityCompat.FinishAfterTransition(this);
        //}
    }
}
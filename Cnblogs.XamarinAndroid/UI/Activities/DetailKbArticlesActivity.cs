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
using Newtonsoft.Json;
using Android.Support.V4.Widget;
using Java.Util.Logging;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "DetailBlogActivity",Theme = "@style/AppTheme")]
    public class DetailKbArticlesActivity : BaseActivity, SwipeRefreshLayout.IOnRefreshListener
    {
        //private Toolbar toolbar;
        private TextView tv_view,tv_ding;
        private WebView wb_content;
        private int ID;//知识库ID
        private Button btn_mark;
        private KbArticles kbArticles;
        private UMengShareWidget shareWidget;
        private SwipeRefreshLayout swipeRefreshLayout;
        private string firstImgUrl;
        protected override int LayoutResourceId => Resource.Layout.detailKbArticles;

        protected override string ToolBarTitle => string.Empty;
        internal static void Enter(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(DetailKbArticlesActivity));
            intent.PutExtra("id",id);
            context.StartActivity(intent);
        }
        internal static void Enter(Context context, int id,KbArticles kb)
        {
            string kbStr = JsonConvert.SerializeObject(kb);
            Intent intent = new Intent(context, typeof(DetailKbArticlesActivity));
            intent.PutExtra("id", id);
            intent.PutExtra("kb", kbStr);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);

            SetToolBarNavBack();
            wb_content = FindViewById<WebView>(Resource.Id.wb_content);
            btn_mark = FindViewById<Button>(Resource.Id.btn_mark);
            tv_view = FindViewById<TextView>(Resource.Id.tv_view);
            tv_ding = FindViewById<TextView>(Resource.Id.tv_ding);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeColors(Resources.GetColor(Resource.Color.primary));
            swipeRefreshLayout.SetOnRefreshListener(this);

            btn_mark.Click += (s, e) =>
            {
                AddBookmarkActivity.Enter(this,string.Format(Constact.KbPage, ID), kbArticles.Title, "add");
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
            ID = Intent.GetIntExtra("id",0);
            if (ID == 0)
            {
                Android.OS.Handler handle = new Android.OS.Handler();
                handle.PostDelayed(() =>
                {
                    Finish();
                }, 2000);
                AlertUtil.ToastShort(this,"获取id错误立即返回");
            }
            InitKbArticle();
            shareWidget = new UMengShareWidget(this);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share,menu);
            return  true;
        }

        async void  InitKbArticle()
        {
            string kbStr = Intent.GetStringExtra("kb");
            if (string.IsNullOrEmpty(kbStr))
            {
                kbArticles = await SQLiteUtil.SelectKbArticles(ID);
            }
            else
            {
                kbArticles = JsonConvert.DeserializeObject<KbArticles>(kbStr);
            }
            if (kbArticles != null)
            {
                OnRefresh();
                tv_view.Text = kbArticles.ViewCount.ToString();
                 tv_ding.Text = kbArticles.Diggcount.ToString();
                 SetToolBarTitle(kbArticles.Title);
            }
        }

        async void GetKbArticleContent(Action callBack)
        {
            try
            {
                var result = await HttpClient.KbArticlesService.GetKbArticles(AccessTokenUtil.GetToken(this),ID);
                if (result.Success)
                {
                    string content = result.Data;
                    firstImgUrl = HtmlUtil.GetHtmlFirstImgUrl(content);
                    content = content.ReplaceHtml().Trim('"');
                    content = HtmlUtil.ReadHtml(Assets).Replace("#body#", content).Replace("#title#",kbArticles.Title).Replace("#author#", "作者：" + kbArticles.Author).Replace("#date#", "发布日期：" + kbArticles.DateAdded.ToString("yyyy年MM月dd日 HH:mm"));
                    wb_content.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                    callBack();
                }
                else
                {
                    AlertUtil.ToastShort(this, result.Message);
                    callBack();
                }
            }
            catch (Exception ex)
            {
                AlertUtil.ToastShort(this,ex.Message);
                callBack();
            }
        }

        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (kbArticles != null)
            {
                // sharesWidget
                string kbArticleUrl = Constact.KbArticleOrigin + "/page" + kbArticles.Id + "/";
                shareWidget.Open(kbArticleUrl,kbArticles.Title,kbArticles.Summary,firstImgUrl);
            }
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode,resultCode,data);
            UMShareAPI.Get(this).OnActivityResult(requestCode,(int)resultCode,data);
        }

        public void OnRefresh()
        {
            swipeRefreshLayout.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
            });
            GetKbArticleContent(()=> {
                swipeRefreshLayout.Post(()=> {
                    swipeRefreshLayout.Refreshing = false;
                });
            });
        }
    }
}
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
using Newtonsoft.Json;
using Android.Support.V4.Widget;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "DetailBlogActivity",Theme = "@style/AppTheme")]
    public class DetailBlogActivity : BaseActivity,SwipeRefreshLayout.IOnRefreshListener
    {
        //private Toolbar toolbar;
        private TextView tv_author, tv_postDate, tv_articleTitle,tv_view,tv_ding;
        private SwipeRefreshLayout swipeRefreshLayout;
        private ImageView iv_avatar;
        private WebView wb_content;
        private int ID;
        DisplayImageOptions options;
        private Button btn_comment, btn_mark;
        private Article article;
        private UMengShareWidget shareWidget;
        private string firstImgSrc;//第一张图片作为分享文章的封面
        protected override int LayoutResourceId => Resource.Layout.detailArticle;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.ToolBar_Title_Blog);
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            ImageLoaderConfiguration configuration =new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//初始化图片加载框架
            ImageLoader.Instance.Init(configuration);
            //显示图片配置
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .CacheOnDisk(true)
                 // .Displayer(new DisplayerImageCircle(20))
                  .Build();
            SetToolBarNavBack();
            ID = Intent.GetIntExtra("id", 0);
            tv_author = FindViewById<TextView>(Resource.Id.tv_author);
            tv_postDate = FindViewById<TextView>(Resource.Id.tv_postDate);
            wb_content = FindViewById<WebView>(Resource.Id.wb_content);
            iv_avatar = FindViewById<ImageView>(Resource.Id.iv_avatar);
            tv_articleTitle = FindViewById<TextView>(Resource.Id.tv_articleTitle);
            btn_comment = FindViewById<Button>(Resource.Id.btn_comment);
            tv_ding = FindViewById<TextView>(Resource.Id.tv_ding);
            btn_mark = FindViewById<Button>(Resource.Id.btn_mark);
            tv_view = FindViewById<TextView>(Resource.Id.tv_view);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeColors(Resources.GetColor(Resource.Color.primary));
            swipeRefreshLayout.SetOnRefreshListener(this);
            btn_mark.Click += (s, e) =>
            {
                AddBookmarkActivity.Enter(this,article.Url,article.Title,"add");
            };

            btn_comment.Click += (s, e) =>
            {
                ArticleCommentActivity.Enter(this, article.BlogApp,ID);
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
            if (ID == 0)
            {
                Android.OS.Handler handle = new Android.OS.Handler();
                handle.PostDelayed(() =>
                {
                    Finish();
                }, 2000);
                AlertUtil.ToastShort(this, "获取id错误立即返回");
            }
            InitArticle();
            shareWidget = new UMengShareWidget(this);
        }


        async void InitArticle()
        {
          
                string articleStr = Intent.GetStringExtra("article");
                if(!string.IsNullOrEmpty(articleStr))//从搜索页过来的
                {
                  article = JsonConvert.DeserializeObject<Article>(articleStr);
                }
                if (article == null)//从博客页面直接点击过来的
                {
                    article = await SQLiteUtil.SelectArticle(ID);
                }
            if (article != null)
            {
                OnRefresh();
                tv_author.Text = article.Author;
                tv_postDate.Text = article.PostDate.ToCommonString();
                tv_articleTitle.Text = article.Title.Replace("\n", "").Replace("  ", "");
                tv_view.Text = article.ViewCount.ToString();
                tv_ding.Text = article.Diggcount.ToString();
                btn_comment.Text = article.CommentCount.ToString();
                if (string.IsNullOrEmpty(article.Avatar)||!article.Avatar.Substring(article.Avatar.Length - 4, 4).Contains(".png"))
                    iv_avatar.SetImageResource(Resource.Drawable.noavatar);
                else
                    ImageLoader.Instance.DisplayImage(article.Avatar, iv_avatar, options);
            }
        }
        async  void GetArticleContent(Action successCallBack)
        {
            try
            {
                var result = await HttpClient.ArticleService.GetArticle(AccessTokenUtil.GetToken(this),ID);
                if (result.Success)
                {
                    string content = result.Data;
                    content = content.ReplaceHtml().Trim('"');
                    content = HtmlUtil.ReadHtml(Assets).Replace("#body#", content).Replace("#title#", "").Replace("#author#", "").Replace("#date#", "");
                    wb_content.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                    firstImgSrc = HtmlUtil.GetHtmlFirstImgUrl(content);
                    successCallBack();
                }
                else
                {
                    AlertUtil.ToastShort(this, result.Message);
                }
            }
            catch (Exception ex)
            {
                AlertUtil.ToastShort(this,ex.Message);
            }
        }
        public void OnRefresh()
        {
            swipeRefreshLayout.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
            });
            GetArticleContent(()=> {
                swipeRefreshLayout.Post(() =>
                {
                    swipeRefreshLayout.Refreshing = false;
                });
            });

        }
        public static void Enter(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(DetailBlogActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }
        public static void Enter(Context context, int id, Article _article)
        {
            Intent intent = new Intent(context, typeof(DetailBlogActivity));
            string articleStr = JsonConvert.SerializeObject(_article);
            intent.PutExtra("id", id);
            intent.PutExtra("article", articleStr);
            context.StartActivity(intent);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share, menu);
            return true;
        }
        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (article != null)
            {
                // sharesWidget
                shareWidget.Open(article.Url,article.Title,article.Description,firstImgSrc);
            }
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode,resultCode,data);
            UMShareAPI.Get(this).OnActivityResult(requestCode,(int)resultCode,data);
        }
    }
}
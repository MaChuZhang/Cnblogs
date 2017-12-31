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
    [Activity(Label = "DetailArticleActivity",Theme = "@style/AppTheme")]
    public class DetailArticleActivity : BaseActivity,SwipeRefreshLayout.IOnRefreshListener
    {
        //private Toolbar toolbar;
        private TextView tv_author, tv_postDate, tv_articleTitle,tv_view;
        private SwipeRefreshLayout swipeRefreshLayout;
        private ImageView iv_avatar;
        private WebView wb_content;
        private int articleId;
        DisplayImageOptions options;
        private Button btn_comment, btn_digg, btn_mark;
        private Article article;
        private UMengShareWidget shareWidget;
        private string firstImgSrc;//��һ��ͼƬ��Ϊ�������µķ���
        protected override int LayoutResourceId => Resource.Layout.detailArticle;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.ToolBar_Title_Blog);
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            ImageLoaderConfiguration configuration =new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//��ʼ��ͼƬ���ؿ��
            ImageLoader.Instance.Init(configuration);
            //��ʾͼƬ����
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
            articleId = Intent.GetIntExtra("id", 0);
            tv_author = FindViewById<TextView>(Resource.Id.tv_author);
            tv_postDate = FindViewById<TextView>(Resource.Id.tv_postDate);
            wb_content = FindViewById<WebView>(Resource.Id.wb_content);
            iv_avatar = FindViewById<ImageView>(Resource.Id.iv_avatar);
            tv_articleTitle = FindViewById<TextView>(Resource.Id.tv_articleTitle);
            btn_comment = FindViewById<Button>(Resource.Id.btn_comment);
            btn_digg = FindViewById<Button>(Resource.Id.btn_digg);
            btn_mark = FindViewById<Button>(Resource.Id.btn_mark);
            tv_view = FindViewById<TextView>(Resource.Id.tv_view);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeColors(Resources.GetColor(Resource.Color.primary));
            swipeRefreshLayout.SetOnRefreshListener(this);
            OnRefresh();
            btn_mark.Click += (s, e) =>
            {
                AddBookmarkActivity.Enter(this,article.Url,article.Title,"add");
            };

            btn_comment.Click += (s, e) =>
            {
                ArticleCommentActivity.Enter(this, article.BlogApp,articleId);
            };
            wb_content.Settings.DomStorageEnabled = true;
            wb_content.Settings.JavaScriptEnabled = true;//֧��js
            wb_content.Settings.DefaultTextEncodingName = "utf-8";//���ñ��뷽ʽutf-8
            wb_content.Settings.SetSupportZoom(false);//��������
            wb_content.Settings.DisplayZoomControls = false;//����ԭ�������ſؼ�
            wb_content.Settings.BuiltInZoomControls = false;//�������õ����ſؼ�
            wb_content.Settings.CacheMode = CacheModes.CacheElseNetwork;
            wb_content.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            wb_content.Settings.LoadsImagesAutomatically = true;//֧���Զ�����ͼƬ
            wb_content.Settings.UseWideViewPort = true; //��ͼƬ����������webview�Ĵ�С
            wb_content.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            var jsInterface = new  WebViewJSInterface(this);
            wb_content.SetWebViewClient(ContentWebViewClient.Instance(this));
            wb_content.AddJavascriptInterface(jsInterface,"openlistner");
            jsInterface.CallFromPageReceived += delegate (object sender,WebViewJSInterface.CallFromPageReceivedEventArgs e)
            {
                  PhotoActivity.Enter(this,e.Result.Split(','),e.Index);
            };
            InitArticle(articleId);
            shareWidget = new UMengShareWidget(this);
        }


        async void InitArticle(int id)
        {
          
                string articleStr = Intent.GetStringExtra("article");
                if(!string.IsNullOrEmpty(articleStr))//������ҳ������
                {
                  article = JsonConvert.DeserializeObject<Article>(articleStr);
                }
                if (article == null)//�Ӳ���ҳ��ֱ�ӵ��������
                {
                    article = await SQLiteUtil.SelectArticle(id);
                    var   result = await SQLiteUtil.SelectArticle(id);
                }
            if (article != null)
            {
                tv_author.Text = article.Author;
                tv_postDate.Text = article.PostDate.ToCommonString();
                tv_articleTitle.Text = article.Title.Replace("\n", "").Replace("  ", "");
                tv_view.Text = article.ViewCount.ToString();
                btn_digg.Text = article.Diggcount.ToString();
                btn_comment.Text = article.CommentCount.ToString();
                if (string.IsNullOrEmpty(article.Avatar)||!article.Avatar.Substring(article.Avatar.Length - 4, 4).Contains(".png"))
                    iv_avatar.SetImageResource(Resource.Drawable.noavatar);
                else
                    ImageLoader.Instance.DisplayImage(article.Avatar, iv_avatar, options);
            }
        }
        void GetFirstImgSrc()
        {
            string[] srcs = HtmlUtil.GetHtmlImageUrlList(article.Content);
            if (srcs.Length > 0)
            {
                firstImgSrc = srcs[0];
            }
            else
                firstImgSrc = "";
        }
        async  void GetArticleContent(Action successCallBack)
        {
            if (articleId==0)
            {
                AlertUtil.ToastShort(this,"δ֪�Ĵ���articleId==0");
                return;
            }
            try
            {
                var result = await HttpClient.ArticleService.GetArticle(AccessTokenUtil.GetToken(this),articleId);
                if (result.Success)
                {
                        article.Content = result.Data;
                        article.Content = article.Content.ReplaceHtml().Trim('"');
                        string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", article.Content).Replace("#title#",article.Title).Replace("#author#", "").Replace("#date#", "");
                        wb_content.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                        GetFirstImgSrc();
                    successCallBack();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void OnRefresh()
        {
            swipeRefreshLayout.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
            });
            GetArticleContent(()=> {
                swipeRefreshLayout.PostDelayed(() =>
                {
                    swipeRefreshLayout.Refreshing = false;
                }, 700);
            });

        }
        public static void Enter(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(DetailArticleActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }
        public static void Enter(Context context, int id, Article _article)
        {
            Intent intent = new Intent(context, typeof(DetailArticleActivity));
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
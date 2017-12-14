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

namespace Cnblogs.XamarinAndroid
{
    [Activity(Theme = "@style/AppTheme")]
    public class DetailNewsActivity : BaseActivity
    {
        //private Toolbar toolbar;
        private TextView tv_view;
        private WebView wb_content;
        private int _id;
        private Button btn_digg;
        private Button btn_mark;
        private Button btn_comment;
        private NewsViewModel news;
        private UMengShareWidget shareWidget;
        
        protected override int LayoutResourceId => Resource.Layout.detailNews;

        protected override string ToolBarTitle => string.Empty;
        internal static void Enter(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(DetailNewsActivity));
            intent.PutExtra("id",id);
            context.StartActivity(intent);
        }
        internal static void Enter(Context context, int id,NewsViewModel model)
        {
            string newsStr = JsonConvert.SerializeObject(model);
            Intent intent = new Intent(context, typeof(DetailNewsActivity));
            intent.PutExtra("id", id);
            intent.PutExtra("news", newsStr);
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
            btn_comment = FindViewById<Button>(Resource.Id.btn_comment);
            tv_view = FindViewById<TextView>(Resource.Id.tv_view);

            btn_mark.Click += (s, e) =>
            {
                AddBookmarkActivity.Enter(this,string.Format(Constact.KbPage, _id), news.Title, "add");
            };
            btn_comment.Click += (s, e) =>
            {
                NewsCommentActivity.Enter(this,_id);
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
            string newsStr = Intent.GetStringExtra("news");
            if (string.IsNullOrEmpty(newsStr))//������ֱ�ӽ����
            {
                news = await SQLiteUtil.SelectNews(id);
            }
            else
            {
                news = JsonConvert.DeserializeObject<NewsViewModel>(newsStr);
            }
            if (news != null)
            {
                 tv_view.Text = news.ViewCount.ToString();
                 btn_digg.Text = news.DiggCount.ToString();
                 btn_comment.Text = news.CommentCount.ToString();
                 SetToolBarTitle(news.Title);
                 if (string.IsNullOrEmpty(news.Body))
                    GetRequestArticle(id);
                 else
                 {
                    news.Body = news.Body.ReplaceHtml().Trim('"');
                    string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", news.Body).Replace("#title#", news.Title).Replace("#author#","").Replace("#date#","�������ڣ�"+news.DateAdded.ToString("yyyy��MM��dd�� HH:mm"));
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
                var result = await HttpClient.NewsService.GetNewsDetail(AccessTokenUtil.GetToken(this),id);
                if (result.Success)
                {
                        news.Body = result.Data;
                        await SQLiteUtil.UpdateNews(news);
                        news.Body = news.Body.ReplaceHtml().Trim('"');
                        string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", news.Body).Replace("#title#",news.Title).Replace("#author#","").Replace("#date#","�������ڣ�"+news.DateAdded.ToString("yyyy��MM��dd�� HH:mm"));
                        wb_content.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (news != null)
            {
                // sharesWidget
                //shareWidget.Open(news.Url,news.Title);
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
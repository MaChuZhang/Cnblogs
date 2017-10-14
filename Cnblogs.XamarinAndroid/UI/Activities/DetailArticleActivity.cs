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

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "DetailArticleActivity",Theme = "@style/BaseAppTheme")]
    public class DetailArticleActivity : AppCompatActivity,Toolbar.IOnMenuItemClickListener,View.IOnClickListener
    {
        private Toolbar toolbar;
        private TextView tv_author, tv_postDate, tv_articleTitle;
        private ImageView iv_avatar;
        private WebView wb_detail;
        private int articleId;
        DisplayImageOptions options;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DetailArticle);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            ImageLoaderConfiguration configuration =new ImageLoaderConfiguration.Builder(this).WriteDebugLogs().Build();//��ʼ��ͼƬ���ؿ��
            ImageLoader.Instance.Init(configuration);
            //��ʾͼƬ����
            options = new DisplayImageOptions.Builder()
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "����";
            toolbar.SetNavigationIcon(Resource.Drawable.icon_back);
            SetSupportActionBar(toolbar);
            tv_author = FindViewById<TextView>(Resource.Id.tv_author);
            tv_postDate = FindViewById<TextView>(Resource.Id.tv_postDate);
            wb_detail = FindViewById<WebView>(Resource.Id.wb_detail);
            iv_avatar = FindViewById<ImageView>(Resource.Id.iv_avatar);
            tv_articleTitle = FindViewById<TextView>(Resource.Id.tv_articleTitle);
            
            wb_detail.Settings.DomStorageEnabled = true;
            wb_detail.Settings.JavaScriptEnabled = true;//֧��js
            wb_detail.Settings.DefaultTextEncodingName = "utf-8";//���ñ��뷽ʽutf-8
            wb_detail.Settings.SetSupportZoom(false);//��������
            wb_detail.Settings.DisplayZoomControls = false;//����ԭ�������ſؼ�
            wb_detail.Settings.BuiltInZoomControls = false;//�������õ����ſؼ�
            wb_detail.Settings.CacheMode = CacheModes.CacheElseNetwork;
            wb_detail.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            wb_detail.Settings.LoadsImagesAutomatically = true;//֧���Զ�����ͼƬ
            wb_detail.Settings.UseWideViewPort = true; //��ͼƬ����������webview�Ĵ�С
            wb_detail.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            articleId = Intent.GetIntExtra("id",0);
            GetClientArticle(articleId);
            toolbar.SetNavigationOnClickListener(this);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share,menu);
            return base.OnCreateOptionsMenu(menu);
        }

        async void GetClientArticle(int id)
        {
            var article = await SQLiteUtil.SelectArticle(id);
            if (article != null)
            {
                tv_author.Text = article.Author;
                tv_postDate.Text = article.PostDate.ToCommonString();
                tv_articleTitle.Text = article.Title;
                if (!article.Avatar.Substring(article.Avatar.Length - 4, 4).Contains(".png"))
                    iv_avatar.SetImageResource(Resource.Drawable.noavatar);
                else
                    ImageLoader.Instance.DisplayImage(article.Avatar, iv_avatar, options);
                if (string.IsNullOrEmpty(article.Content))
                    GetRequestArticle(id);
                else
                {
                    article.Content = article.Content.ReplaceHtml().Trim('"');
                    string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", article.Content).Replace("#title#", article.Title);
                    wb_detail.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
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
                var result = await HttpClient.ArticleRequest.GetArticleDetail(SharedDataUtil.GetToken(this),id);
                if (result.Success)
                {
                    await SQLiteUtil.SelectArticle(id).ContinueWith(async (r) =>
                    {
                        var article = r.Result;
                        article.Content = result.Data;
                        await SQLiteUtil.UpdateArticle(article);
                        article.Content = article.Content.ReplaceHtml().Trim('"');
                        string content = HtmlUtil.ReadHtml(Assets).Replace("#body#", article.Content).Replace("#title#",article.Title);
                        wb_detail.LoadDataWithBaseURL("file:///android_asset/", content, "text/html", "utf-8", null);
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}
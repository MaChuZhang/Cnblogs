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

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "DetailArticleActivity",Theme = "@style/BaseAppTheme")]
    public class DetailArticleActivity : AppCompatActivity
    {
        private Toolbar toolbar;
        private TextView tv_author, tv_postDate, tv_articleTitle;
        private ImageView iv_avatar;
        private WebView wb_detail;
        private int articleId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DetailArticle);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "博客";
            SetSupportActionBar(toolbar);
            tv_author = FindViewById<TextView>(Resource.Id.tv_author);
            tv_postDate = FindViewById<TextView>(Resource.Id.tv_postDate);
            wb_detail = FindViewById<WebView>(Resource.Id.wb_detail);
            iv_avatar = FindViewById<ImageView>(Resource.Id.iv_avatar);
            tv_articleTitle = FindViewById<TextView>(Resource.Id.tv_articleTitle);
            wb_detail.Settings.DomStorageEnabled = true;
            wb_detail.Settings.JavaScriptEnabled = true;//支持js
            wb_detail.Settings.DefaultTextEncodingName = "utf-8";//设置编码方式utf-8
            wb_detail.Settings.SetSupportZoom(false);//不可缩放
            wb_detail.Settings.DisplayZoomControls = false;//隐藏原生的缩放控件
            wb_detail.Settings.BuiltInZoomControls = false;//设置内置的缩放控件
            wb_detail.Settings.CacheMode = CacheModes.CacheElseNetwork;
            wb_detail.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            wb_detail.Settings.LoadsImagesAutomatically = true;//支持自动加载图片
            wb_detail.Settings.UseWideViewPort = true; //将图片调整到合适webview的大小
            wb_detail.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            articleId = Intent.GetIntExtra("id",0);
            GetClientArticle(articleId);
        }

        async void GetClientArticle(int id)
        {
            var article = await SQLiteUtil.SelectArticle(id);
            if (article != null)
            {
                tv_author.Text = article.Author;
                tv_postDate.Text = article.PostDate.ToCommonString();
                tv_articleTitle.Text = article.Title;
                if (string.IsNullOrEmpty(article.Content))
                    GetRequestArticle(id);
                else
                {
                    wb_detail.LoadDataWithBaseURL("file:///android asset/", article.Content.Trim('"').ReplaceHtml(), "text/html", "utf-8", null);
                }
            }
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
                        wb_detail.LoadDataWithBaseURL("file:///android asset/",article.Content.Trim('"').ReplaceHtml(), "text/html","utf-8",null);
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
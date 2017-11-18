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

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "AddBookmarkActivity", Theme = "@style/AppTheme")]
    public class AddBookmarkActivity : BaseActivity
    {
        protected override int LayoutResourceId => Resource.Layout.addBookmark;

        protected override string ToolBarTitle => Resources.GetString(Resource.String.bookmark_add);
        private string tv_url;
        private string tv_title;
        internal static void Enter(Context context, string url, string title)
        {
            Intent intent = new Intent(context, typeof(AddBookmarkActivity));
            intent.PutExtra("url", url);
            intent.PutExtra("title", title);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            StatusBarUtil.SetColorStatusBars(this);
            SetNavIcon(Resource.Drawable.icon_back);
            
            string  title = Intent.GetStringExtra("title");
            string url = Intent.GetStringExtra("url");
        }

        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (article != null)
            {
                // sharesWidget
                shareWidget.Open(article.Url,article.Title);
            }
            return true;
        }

        //public void OnClick(View v)
        //{
        //    ActivityCompat.FinishAfterTransition(this);
        //}
    }
}
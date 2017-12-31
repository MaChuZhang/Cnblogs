using Android.App;
using Android.Widget;
using Android.OS;
using Android.Text;
using Com.Umeng.Socialize.Bean;
using Java.Lang;
using Com.Umeng.Socialize.Media;
using Com.Umeng.Socialize;
using Android.Content;
using Com.Umeng.Socialize.Utils;
using Com.Umeng.Socialize.Shareboard;

namespace UMengTest
{
    [Activity(Label = "UMengTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            // SetContentView (Resource.Layout.Main);
            TextView Text017_01 = FindViewById<TextView>(Resource.Id.Text017_01);
            Text017_01.Click += (s, e) =>
            {  
                UMengShareWidget shareWidget = new UMengTest.UMengShareWidget(this);
                shareWidget.Open("https://www.jianshu.com/p/eeb378b99bf3","测试标题","测试简介",null);
            };
        }
    }


    public class UMengShareWidget : Java.Lang.Object, IShareBoardlistener, IUMShareListener
    {
        private Activity context;
        private ShareAction shareAction;
        private UMWeb umWeb;
        public UMengShareWidget(Activity context)
        {
            this.context = context;
            shareAction = new ShareAction(context)
                .SetDisplayList(SHARE_MEDIA.Weixin, SHARE_MEDIA.WeixinCircle, SHARE_MEDIA.Qq, SHARE_MEDIA.Qzone)
                .AddButton("微信", "微信朋友圈", "QQ", "QQ空间")
                .SetShareboardclickCallback(this);
        }
        //IShareBoardListener
        public void Onclick(SnsPlatform snsPlatform, SHARE_MEDIA media)
        {
            if (snsPlatform.MShowWord.Equals("浏览器打开"))
            {
                try
                {
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetData(Android.Net.Uri.Parse(umWeb.ToUrl()));
                    context.StartActivity(intent);
                }
                catch (System.Exception e)
                {
                    Toast.MakeText(context, "浏览器打开失败", ToastLength.Short);
                }
            }
            else
            {
                new ShareAction(context).WithMedia(umWeb)
                    .SetPlatform(media)
                    .SetCallback(this)
                    .Share();
            }
        }
        public void Open(string url, string title, string desc, object icon = null)
        {
            umWeb = new UMWeb(url);
            umWeb.Title = title;
            umWeb.Description = desc;
            if (icon==null||string.IsNullOrWhiteSpace(icon.ToString()))
            {
                umWeb.SetThumb(new UMImage(context, Resource.Drawable.Icon));
            }
            else
            {
                umWeb.SetThumb(new UMImage(context, icon.ToString()));
            }
            shareAction.Open();
        }
        public void Close()
        {
            shareAction.Close();
        }
        //IUMShareListener
        public void OnCancel(SHARE_MEDIA p0)
        {
            //throw new NotImplementedException();
            shareAction.Close();
        }

        public void OnError(SHARE_MEDIA p0, Throwable p1)
        {
            //.ToastShort(context, "分享失败了")
            Toast.MakeText(context, "分享失败了",ToastLength.Short);
        }


        public void OnResult(SHARE_MEDIA media)
        {
            if (media == SHARE_MEDIA.WeixinFavorite)
            {
                Toast.MakeText(context, "收藏失败了", ToastLength.Short);
            }
            else
            {
                Toast.MakeText(context, "收藏成功了", ToastLength.Short);
            }
        }

        public void OnStart(SHARE_MEDIA p0)
        {
            //throw new NotImplementedException();
        }


    }


}


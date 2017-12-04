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
using Com.Umeng.Socialize.Utils;
using Com.Umeng.Socialize;
using Com.Umeng.Socialize.Bean;
using Com.Umeng.Socialize.Shareboard;
using Java.Lang;
using Com.Umeng.Socialize.Media;

namespace Cnblogs.XamarinAndroid.UI.Widgets
{
    public class UMengShareWidget: Java.Lang.Object,IShareBoardlistener,IUMShareListener
    {
        private Activity context;
        private ShareAction shareAction;
        private UMWeb umWeb;
        public  UMengShareWidget(Activity context)
        {
            this.context = context;
            shareAction = new ShareAction(context)
                .SetDisplayList(SHARE_MEDIA.Weixin, SHARE_MEDIA.WeixinCircle,SHARE_MEDIA.Qq,SHARE_MEDIA.Qzone)
                .AddButton("΢��", "΢������Ȧ","QQ","QQ�ռ�")
                .SetShareboardclickCallback(this);
        }
        //IShareBoardListener
        public void Onclick(SnsPlatform snsPlatform, SHARE_MEDIA media)
        {
            if (snsPlatform.MShowWord.Equals("�������"))
            {
                try
                {
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetData(Android.Net.Uri.Parse(umWeb.ToUrl()));
                    context.StartActivity(intent);
                }
                catch (System.Exception e)
                {
                    AlertUtil.ToastShort(context, "�������ʧ��");
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
        public void Open(string  url,string  title,object icon =null)
        {
            umWeb = new UMWeb(url);
            umWeb.Title = title;
            if (icon == null)
            {
                umWeb.SetThumb(new UMImage(context, Resource.Drawable.icon_app));
            }
            else
            {
                umWeb.SetThumb(new UMImage(context,icon.ToString()));
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
            AlertUtil.ToastShort(context,"����ʧ����");
        }

        public void OnResult(SHARE_MEDIA media)
        {
            if (media == SHARE_MEDIA.WeixinFavorite)
            {
                AlertUtil.ToastShort(context, "�ղسɹ�");
            }
            else
            {
                AlertUtil.ToastShort(context, "����ɹ�");
            }
        }

        public void OnStart(SHARE_MEDIA p0)
        {
            //throw new NotImplementedException();
        }

        
    }
}
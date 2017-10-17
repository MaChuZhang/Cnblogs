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
using Android.Webkit;

namespace Cnblogs.XamarinAndroid
{
    public class ContentWebViewClient:WebViewClient
    {
        private volatile static ContentWebViewClient instance;
        private Context context;

        public ContentWebViewClient(Context context)
        {
            this.context = context;
        }

        public static ContentWebViewClient Instance(Context context)
        {
            if (instance == null)
            {
                instance = new ContentWebViewClient(context);
            }
            return instance;
        }
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.LoadUrl("javascript:(function(){" +
                                    "var imgs = document.querySelectorAll(\"img\"); " +
                                    "var srcs=new Array();" +
                                    "for(var i=0;i<imgs.length;i++)  " +
                                    "{" +
                                    "    srcs.push(imgs[i].src);" +
                                    "    imgs[i].onclick=(function(index)" +
                                    "    {  " +
                                    "     return function(){   openlistner.OpenImage(srcs.toString(),index);}  " +
                                    "    })(i);  " +
                                    "};" +
                                    "})()");
        }

    }
}
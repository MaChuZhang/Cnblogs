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
    public class WebViewJSInterface : Java.Lang.Object
    {
        Context context { get; set; }

        public WebViewJSInterface(Context context)
        {
            this.context = context;
        }

        [Java.Interop.Export]
        [JavascriptInterface]
        public void OpenImage(string srcs, int index)
        {
            CallFromPageReceived?.Invoke(this, new CallFromPageReceivedEventArgs
            {
                Result = srcs,
                Index = index
            });
        }
        [Java.Interop.Export]
        [JavascriptInterface]
        public void OpenHref(string href)
        {
            CallFromPageReceived?.Invoke(this, new CallFromPageReceivedEventArgs
            {
                Result = href,
                Index = 0
            });
        }

        public event EventHandler<CallFromPageReceivedEventArgs> CallFromPageReceived;
        public class CallFromPageReceivedEventArgs : EventArgs
        {
            public string Result { get; set; }
            public int Index { get; set; }
        }
    }
}
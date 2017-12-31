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
using Com.Umeng.Socialize;

namespace UMengTest
{
    public class MyApplication:Application
    {
        public override void OnCreate()
        {
            UMShareAPI.Get(this);
            Config.Debug = false;
            PlatformConfig.SetWeixin("wx633888643fbae319", "5034ad765b2ba64dec7ed7c6618581fb");
            PlatformConfig.SetQQZone("100424468", "c7394704798a158208a74ab60104f0ba");
            base.OnCreate();
        }
        public MyApplication() { }
        public MyApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
    }
}
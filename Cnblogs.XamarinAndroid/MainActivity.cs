﻿using Android.App;
using Android.Widget;
using Android.OS;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "Cnblogs.XamarinAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}


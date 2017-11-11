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
using static Android.App.ActionBar;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.Widget;

namespace Cnblogs.XamarinAndroid.UI.Activities
{
    [Activity(Label = "TestActivity",MainLauncher =false,Theme ="@style/AppTheme")]
    public class TestActivity :AppCompatActivity
    {
        private Button btn_tabStatus,btn_test;
        private Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout1);
           toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            btn_tabStatus =toolbar.FindViewById<Button>(Resource.Id.btn_tabStatus);
            //btn_test =toolbar.FindViewById<Button>(Resource.Id.btn_test);



            btn_tabStatus.Click += (s,e)=> 
            {
                AlertUtil.ToastShort(this,"123");
            };

        }
    }
}
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Text;
using Java.Lang;
using Android.Content;

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
        }
    }





}


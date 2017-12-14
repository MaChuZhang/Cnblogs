using Android.App;
using Android.Widget;
using Android.OS;
using Android.Text;

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
                FindViewById<TextView>(Resource.Id.TextCommonSpecimen).Text = "Text017_01acdascdasc茶市场大事超大市场大市场打算从超大市场大市场打算从大神查收超大市场大市场";
            };
        }
    }
}


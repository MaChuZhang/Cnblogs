using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V4.View;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/myStatus", Theme = "@style/AppTheme")]
    public class MyStatusActivity : BaseActivity, TabLayout.IOnTabSelectedListener
    {
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.myStatus;
            }
        }
        private DisplayImageOptions options;
        private ViewPager _viewPager;
        private TabLayout _tab;
        private StatusTabsFragmentAdapter adapter;
        internal static void Enter(string _blogApp, Context context)
        {
            Intent intent = new Intent(context, typeof(MyStatusActivity));
            intent.PutExtra("blogApp", _blogApp);
            context.StartActivity(intent);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetNavIcon(Resource.Drawable.back_24dp);
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.myStatus));
            //œ‘ æÕº∆¨≈‰÷√
            options = new DisplayImageOptions.Builder()
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.icon_user)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheOnDisk(true)
                  .Displayer(new DisplayerImageCircle(20))
                  .Build();
            _viewPager = FindViewById<ViewPager>(Resource.Id.viewPager_home);
            _tab = FindViewById<TabLayout>(Resource.Id.tab_home);
            string[] myStatusTabs = Resources.GetStringArray(Resource.Array.MyStatusTabs);
            adapter = new StatusTabsFragmentAdapter(SupportFragmentManager, myStatusTabs,true);

            _viewPager.Adapter = adapter;
            _viewPager.OffscreenPageLimit = myStatusTabs.Length;
            _tab.TabMode = TabLayout.ModeFixed;
            _tab.SetupWithViewPager(_viewPager);
            _tab.SetOnTabSelectedListener(this);
        }

        public void OnTabReselected(TabLayout.Tab tab)
        {
            //throw new NotImplementedException();
        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            _viewPager.CurrentItem = tab.Position;
        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
           // throw new NotImplementedException();
        }
    }
}
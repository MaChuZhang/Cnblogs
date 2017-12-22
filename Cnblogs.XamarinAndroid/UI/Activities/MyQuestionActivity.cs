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
using Cnblogs.XamarinAndroid.Adapter;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "@string/myQuestion", Theme = "@style/AppTheme")]
    public class MyQuestionActivity : BaseActivity, TabLayout.IOnTabSelectedListener
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
        private QuestionTabFragmentAdapter adapter;
        internal static void Enter(string _blogApp, Context context)
        {
            Intent intent = new Intent(context, typeof(MyQuestionActivity));
            intent.PutExtra("blogApp", _blogApp);
            context.StartActivity(intent);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetToolBarNavBack();
            StatusBarUtil.SetColorStatusBars(this);
            SetToolBarTitle(Resources.GetString(Resource.String.myStatus));
            //œ‘ æÕº∆¨≈‰÷√
            options = new DisplayImageOptions.Builder()
                .ShowImageForEmptyUri(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnFail(Resource.Drawable.icon_yuanyou)
                  .ShowImageOnLoading(Resource.Drawable.icon_user)
                  .CacheInMemory(true)
                  .BitmapConfig(Bitmap.Config.Rgb565)
                  .CacheOnDisk(true)
                 // .Displayer(new DisplayerImageCircle(20))
                  .Build();
            _viewPager = FindViewById<ViewPager>(Resource.Id.viewPager_home);
            _tab = FindViewById<TabLayout>(Resource.Id.tab_home);
            string[] myQuestionTabs = Resources.GetStringArray(Resource.Array.MyQuestionTabs);
            adapter = new QuestionTabFragmentAdapter(SupportFragmentManager, myQuestionTabs, true);

            _viewPager.Adapter = adapter;
            _viewPager.OffscreenPageLimit = myQuestionTabs.Length;
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.add, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnMenuItemClick(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.add)
            {
                AddQuestionActivity.Enter(this);
            }
            return base.OnMenuItemClick(item);
        }
    }
}
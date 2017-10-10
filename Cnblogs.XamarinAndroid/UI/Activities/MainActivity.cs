using Android.App;
using Android.Widget;
using Android.OS;
using Cnblogs.XamarinAndroid.UI.Activities;
using System;
using Android.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Cnblogs.XamarinAndroid.Util;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Cnblogs.XamarinAndroid;

using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Cnblogs.HttpClient;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "Cnblogs.XamarinAndroid", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/BaseAppTheme")]
    public class MainActivity : BaseActivity
    {
        private DateTime ? firstBackTime; //第一次单击返回
        private Toolbar _toolbar;
        private DrawerLayout _drawerLayout;
        private ActionBarDrawerToggle _drawerToggle;
        private NavigationView _navigationView;

        private NewsFragment _newsFragment;
        private HomeFragment _homeFragment;
        private KbArticlesFragment _kbArticlesFragment;
        private FragmentManager _fm;
        private TextView tv_news,tv_kbArticles,tv_qa,tv_shancun;
        protected override int LayoutResourceId
        {
            get
            {
                return Resource.Layout.Main;
            }
        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StatusBarUtil.SetColorStatusBars(this);
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
           // _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
          //  _navigationView = FindViewById<NavigationView>(Resource.Id.navigationView);
            _fm = SupportFragmentManager;

            _toolbar.Title = Resources.GetString(Resource.String.CnblogsTitle);
            tv_news = FindViewById<TextView>(Resource.Id.tv_news);
            tv_qa = FindViewById<TextView>(Resource.Id.tv_qa);
            tv_shancun = FindViewById<TextView>(Resource.Id.tv_shancun);
            tv_kbArticles = FindViewById<TextView>(Resource.Id.tv_kbArticles);
            tv_news.Selected = true;
            BindViewsClick();
            //  SetSupportActionBar(_toolbar);
            // SupportActionBar.SetDisplayShowHomeEnabled(true);
            // SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //_drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, _toolbar, 0, 0);
            //_drawerLayout.SetDrawerListener(_drawerToggle);
            // _drawerToggle.SyncState();

            //_navigationView.InflateMenu(Resource.Menu.main);
            // _navigationView.InflateHeaderView(Resource.Layout.navheader);
            // _navigationView.NavigationItemSelected += (s, e) =>
            // {
            //      NavItemSwitch(e.MenuItem.ItemId);
            // };
        }
        void SetUnSelected()
        {
            tv_news.Selected = false;
            tv_kbArticles.Selected = false;
            tv_qa.Selected = false;
            tv_shancun.Selected = false;
        }
        //隐藏所有Fragment
        void hideAllFragment(FragmentTransaction fragmentTransaction)
        {
            if (_homeFragment != null) fragmentTransaction.Hide(_homeFragment);
            if (_kbArticlesFragment != null) fragmentTransaction.Hide(_kbArticlesFragment);
            if (_newsFragment != null) fragmentTransaction.Hide(_newsFragment);
        }
        void BindViewsClick()
        {
            tv_qa.Click += (s, e) => { MenuSwitch(tv_qa); };
            tv_shancun.Click += (s, e) => { MenuSwitch(tv_shancun); };
            tv_kbArticles.Click += (s, e) => { MenuSwitch(tv_kbArticles); };
            tv_news.Click += (s, e) => { MenuSwitch(tv_news); };
        }
        void MenuSwitch(View v)
        {
            FragmentTransaction ft = _fm.BeginTransaction();
            hideAllFragment(ft);
            switch (v.Id)
            {
                case Resource.Id.tv_news:
                    SetUnSelected();
                    tv_news.Selected = true;
                    if (_homeFragment == null)
                    {
                        _homeFragment = new HomeFragment();
                        ft.Add(Resource.Id.frameContent, _homeFragment);
                    }
                    else ft.Show(_homeFragment);
                    break;

                case Resource.Id.tv_kbArticles:
                    SetUnSelected();
                    tv_kbArticles.Selected = true;
                    if (_kbArticlesFragment == null)
                    {
                        _kbArticlesFragment = new KbArticlesFragment();
                        ft.Add(Resource.Id.frameContent, _kbArticlesFragment);
                    }
                    else ft.Show(_kbArticlesFragment);
                        break;
                case Resource.Id.tv_qa:
                    SetUnSelected();
                    tv_qa.Selected = true;
                    if (_newsFragment == null)
                    {
                        _newsFragment = new NewsFragment();
                        ft.Add(Resource.Id.frameContent,_newsFragment);
                    }
                    else
                        ft.Show(_newsFragment);
                        break;
            }
            ft.Commit();
        }
        public override bool OnKeyDown(Keycode keycode , KeyEvent e)
        {
            if (keycode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                if (!firstBackTime.HasValue || DateTime.Now.Second - firstBackTime.Value.Second > 2)
                {
                    AlertUtil.ToastShort(this, "再按一次退出");
                    firstBackTime = DateTime.Now;
                }
                else
                {
                    this.Finish();
                }
                return true;
            }
            return base.OnKeyDown(keycode,e);
        }
    }
}


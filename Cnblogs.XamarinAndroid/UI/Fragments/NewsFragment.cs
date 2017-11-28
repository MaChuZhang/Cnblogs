using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.Design.Widget;
using Cnblogs.HttpClient;
using Cnblogs.ApiModel;
using Com.Nostra13.Universalimageloader.Core;
using Android.Graphics;

namespace Cnblogs.XamarinAndroid
{
    public class NewsFragment : Fragment, TabLayout.IOnTabSelectedListener
    {
        private ViewPager _viewPagerHome;
        private TabLayout _tabHome;
        private NewsFragmentTabsAdapter adapter;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //Activity.MenuInflater.Inflate(Resource.Menu.search, menu);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            HasOptionsMenu = true;
            _viewPagerHome = view.FindViewById<ViewPager>(Resource.Id.viewPager_home);
            _tabHome = view.FindViewById<TabLayout>(Resource.Id.tab_home);

            //List<string> list = new List<string>() { "最新","精华"};

            adapter = new NewsFragmentTabsAdapter(this.ChildFragmentManager, Resources.GetStringArray(Resource.Array.NewsTabs));

            _viewPagerHome.Adapter = adapter;
            _tabHome.TabMode = TabLayout.GravityCenter;
            _tabHome.SetupWithViewPager(_viewPagerHome);
            _tabHome.SetOnTabSelectedListener(this);

        }

        public void OnTabReselected(TabLayout.Tab tab)
        {

        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            _viewPagerHome.CurrentItem = tab.Position;

        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {

        }
    }
}
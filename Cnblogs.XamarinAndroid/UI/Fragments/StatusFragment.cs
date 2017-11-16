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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using static Android.App.ActionBar;

namespace Cnblogs.XamarinAndroid
{
    public class StatusFragment : Fragment, TabLayout.IOnTabSelectedListener
    {
        private ViewPager _viewPager;
        private TabLayout _tab;
        private StatusTabsFragmentAdapter adapter;
        private Button btn_status,btn_question;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            btn_status = Activity.FindViewById<Button>(Resource.Id.btn_tabStatus);
            btn_question = Activity.FindViewById<Button>(Resource.Id.btn_tabQuestion);
        
            btn_status.Click += (s, e) =>
            {
                if (btn_status.Tag!=null &&!(bool)btn_status.Tag)
                    return;
               // btn_status.SetBackgroundColor(Resources.GetColor(Resource.Color.primaryDark));
                btn_status.Background=Resources.GetDrawable(Resource.Drawable.shape_corner_left_selected);
                btn_status.SetTextColor(Resources.GetColor(Resource.Color.white));
                btn_status.SetTypeface(Android.Graphics.Typeface.SansSerif,Android.Graphics.TypefaceStyle.Bold);

                btn_question.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_right);
                btn_question.SetTextColor(Resources.GetColor(Resource.Color.black));
                btn_question.SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);

                btn_status.Tag = false;
                btn_question.Tag = true;
            };
            btn_question.Click += (s, e) =>
            {
                if (btn_question.Tag!=null&&!(bool)btn_question.Tag)
                    return;

                btn_question.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_right_selected);
                btn_question.SetTextColor(Resources.GetColor(Resource.Color.white));
                btn_question.SetTypeface(Android.Graphics.Typeface.SansSerif, Android.Graphics.TypefaceStyle.Bold);

                btn_status.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_left);
                btn_status.SetTextColor(Resources.GetColor(Resource.Color.black));
                btn_status.SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);

                btn_status.Tag = true;
                btn_question.Tag = false;
            };
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            Activity.MenuInflater.Inflate(Resource.Menu.add, menu);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {  
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            return   inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }
        public override void OnViewCreated(View view,Bundle savedInstanceState)
        {
            base.OnViewCreated(view,savedInstanceState);
            HasOptionsMenu=true;
            btn_status.PerformClick();
            _viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager_home);
            _tab = view.FindViewById<TabLayout>(Resource.Id.tab_home);
            string[] statusTabs = Resources.GetStringArray(Resource.Array.StatusTabs);
            adapter =new StatusTabsFragmentAdapter(this.ChildFragmentManager, statusTabs);

            _viewPager.Adapter = adapter;
            _viewPager.OffscreenPageLimit = statusTabs.Length ;
            _tab.TabMode = TabLayout.ModeFixed;
            _tab.SetupWithViewPager(_viewPager);
            _tab.SetOnTabSelectedListener(this);

        }

        public void OnTabReselected(TabLayout.Tab tab)
        {
            
        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            _viewPager.CurrentItem = tab.Position;
            
        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
            
        }
    }
}
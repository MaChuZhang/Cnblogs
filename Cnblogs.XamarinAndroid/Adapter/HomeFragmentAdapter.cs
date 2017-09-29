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
using Android.Support.V4.App;

namespace Cnblogs.XamarinAndroid
{
    public class HomeFragmentTabsAdapter :FragmentPagerAdapter
    {
        private const int TabItemCount = 2;
        private Android.Support.V4.App.FragmentManager _fragmentManager;
        private List<RecyclerViewFragment> fragments=new List<RecyclerViewFragment> ();
        private string[] tabsTitle;
        public HomeFragmentTabsAdapter(Android.Support.V4.App.FragmentManager fm,string[] tabsTitle):base(fm)
        {
            _fragmentManager = fm;
            this.tabsTitle = tabsTitle;
        }
        public override int Count
        {
            get
            {
                return tabsTitle.Length;
            }
        }
        public new string GetPageTitle(int position)
        {
            return tabsTitle[position];
        }
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0)
        {
            return new Java.Lang.String(tabsTitle[p0]);
        }
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            var fragment = RecyclerViewFragment.Instance(position);
            fragments.Add(fragment);
            return fragment;
        }
    }
}
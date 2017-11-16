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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.App;

namespace Cnblogs.XamarinAndroid
{
    [Activity]
    public abstract class BaseActivity : AppCompatActivity,View.IOnClickListener, Toolbar.IOnMenuItemClickListener
    {
        private Toolbar toolbar;
        private LinearLayout ly_tabs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarUtil.SetColorStatusBars(this);
            SetContentView(LayoutResourceId);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if(toolbar.ChildCount>0)
            { 
                ly_tabs = FindViewById<LinearLayout>(Resource.Id.ly_tabs);
            }
            toolbar.Title = ToolBarTitle;
        
            SetSupportActionBar(toolbar);
            toolbar.SetOnMenuItemClickListener(this);
            // Create your application here
        }
        protected void SetTabVisible(bool isVisible)
        {
            if (ly_tabs != null)
            {
                if (isVisible)
                    ly_tabs.Visibility = ViewStates.Visible;
                else
                    ly_tabs.Visibility = ViewStates.Gone;
            }
        }
        protected abstract int LayoutResourceId
        {
            get;
        }
        protected virtual string ToolBarTitle {
            get;
        }
        protected void SetToolBarTitle(string title) {
            this.toolbar.Title = title;
        }
        protected virtual void SetNavIcon(int resId)
        {
            toolbar.SetNavigationIcon(resId);
            toolbar.SetNavigationOnClickListener(this);
        }

        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }

        public virtual bool OnMenuItemClick(IMenuItem item)
        {
            // throw new NotImplementedException();
            return true;
        }
    }
}
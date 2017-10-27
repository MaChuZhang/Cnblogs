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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarUtil.SetColorStatusBars(this);
            SetContentView(LayoutResourceId);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = ToolBarTitle;
        
            SetSupportActionBar(toolbar);
            toolbar.SetOnMenuItemClickListener(this);
            // Create your application here
        }
        protected abstract int LayoutResourceId
        {
            get;
        }
        protected abstract string ToolBarTitle {
            get;
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
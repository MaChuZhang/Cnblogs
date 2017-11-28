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
    public abstract class BaseActivity : AppCompatActivity, Toolbar.IOnMenuItemClickListener
    {
        private Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarUtil.SetColorStatusBars(this);
            SetContentView(LayoutResourceId);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                toolbar.Title = ToolBarTitle;

                SetSupportActionBar(toolbar);
                toolbar.SetOnMenuItemClickListener(this);
            }
            // Create your application here
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
        protected virtual void SetToolBarNavBack()
        {
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            toolbar.NavigationClick += (s, e) =>
            {
                ActivityCompat.FinishAfterTransition(this);
            };
        }

        protected virtual void SetToolBarNavUserCenter()
        {
            toolbar.SetNavigationIcon(Resource.Drawable.icon_user_nav);
            toolbar.NavigationClick += (s, e) =>
            {
                StartActivity(new Intent(this, typeof(UserCenterActivity)));
            };
        }

        //public void OnClick(View v)
        //{
            
        //    if (!isBack)
        //    {
        //        context.StartActivity(new Intent(context, typeof(UserCenterActivity)));
        //        //AlertUtil.ToastShort(this,"跳转到我的个人中心");
        //    }
        //    if(isBack)
        //       ActivityCompat.FinishAfterTransition(this);
        //}

        public virtual bool OnMenuItemClick(IMenuItem item)
        {
            // throw new NotImplementedException();
            return true;
        }
    }
}
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
using Android.Support.V7.Widget;

namespace Cnblogs.XamarinAndroid
{
    public  class RecyclerviewScroll:RecyclerView.OnScrollListener
    {
        //private static int hide_threshold = 20;
        //private int scrollDistance = 0;
        //private bool controlsVisible = true;
        internal delegate void Hide();
        internal delegate void Show();
        internal Hide OnHide;
        internal Show OnShow;
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            //if (scrollDistance > hide_threshold && controlsVisible)
            //{
            //    OnHide();
            //    controlsVisible = false;
            //    scrollDistance = 0;
            //}
            //else if (scrollDistance<-hide_threshold&&!controlsVisible)
            //{
            //    OnShow();
            //    controlsVisible = true;
            //    scrollDistance = 0;
            //}
            //if (controlsVisible && dy > 0 || (!controlsVisible && dy < 0))
            //{
            //    scrollDistance += dy;
            //}
            if (dy > 0)
                OnHide();
            else
                OnShow();
        }
    }
}
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
using Android.Support.V4.View;
using Android.Graphics;

namespace Cnblogs.XamarinAndroid
{
    class StatusBarUtil
    {
            private static View _statusBarView;
            /// <summary>
            /// 设置颜色状态栏
            /// </summary>
            /// <param name="activity"></param>
            public static void SetColorStatusBars(Activity activity)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var color = activity.Resources.GetColor(Resource.Color.primary);
                    //清除透明状态栏，使内容不再覆盖状态栏  
                    activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    activity.Window.SetStatusBarColor(color);
                    //透明导航栏部分手机导航栏不是虚拟的
                    //activity.Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                    activity.Window.SetNavigationBarColor(color);
                }
                else if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat)
                {
                    SetKKStatusBar(activity, Resource.Color.primary);
                }
            }
            //设置透明状态栏，android4.4以上都支持透明化状态
            public static void SetTransparentStausBar(Activity activity)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    //状态栏透明  
                    activity.Window.AddFlags(WindowManagerFlags.TranslucentStatus);
                    //透明导航栏  
                    activity.Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }
            }
            private static void SetKKStatusBar(Activity activity, int statusBarColor)
            {
               SetTransparentStausBar(activity);//先透明化("去掉"状态栏)
               ViewGroup contentView = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
                _statusBarView = contentView.GetChildAt(0);
                 Color primaryColor = activity.Resources.GetColor(statusBarColor);
                //防止重复添加statusBarView
                if (_statusBarView != null && _statusBarView.LayoutParameters.Height == GetStatusBarHeight(activity))
                {
                    _statusBarView.SetBackgroundColor(primaryColor);
                    return;
                }
               //if (_statusBarView != null)
               // {
               //    ViewCompat.SetFitsSystemWindows(_statusBarView,false);
               // }
                _statusBarView = new View(activity);
                ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, GetStatusBarHeight(activity));
                _statusBarView.SetBackgroundColor(primaryColor);//填充的到状态栏的view设置颜色
                contentView.AddView(_statusBarView,0, lp);
            }
            private static int GetStatusBarHeight(Context context)
            {
                int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                return context.Resources.GetDimensionPixelSize(resourceId);
            }
        }
}
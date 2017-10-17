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
            /// ������ɫ״̬��
            /// </summary>
            /// <param name="activity"></param>
            public static void SetColorStatusBars(Activity activity)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var color = activity.Resources.GetColor(Resource.Color.primary);
                    //���͸��״̬����ʹ���ݲ��ٸ���״̬��  
                    activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    activity.Window.SetStatusBarColor(color);
                    //͸�������������ֻ����������������
                    //activity.Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                    activity.Window.SetNavigationBarColor(color);
                }
                else if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat)
                {
                    SetKKStatusBar(activity, Resource.Color.primary);
                }
            }
            //����͸��״̬����android4.4���϶�֧��͸����״̬
            public static void SetTransparentStausBar(Activity activity)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    //״̬��͸��  
                    activity.Window.AddFlags(WindowManagerFlags.TranslucentStatus);
                    //͸��������  
                    activity.Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }
            }
            private static void SetKKStatusBar(Activity activity, int statusBarColor)
            {
               SetTransparentStausBar(activity);//��͸����("ȥ��"״̬��)
               ViewGroup contentView = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
                _statusBarView = contentView.GetChildAt(0);
                 Color primaryColor = activity.Resources.GetColor(statusBarColor);
                //��ֹ�ظ����statusBarView
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
                _statusBarView.SetBackgroundColor(primaryColor);//���ĵ�״̬����view������ɫ
                contentView.AddView(_statusBarView,0, lp);
            }
            private static int GetStatusBarHeight(Context context)
            {
                int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                return context.Resources.GetDimensionPixelSize(resourceId);
            }
        }
}
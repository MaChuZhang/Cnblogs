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

namespace Cnblogs.XamarinAndroid
{
    class StatusBarUtil
    {
            private static View _statusBarView;
            /// <summary>
            /// ������ɫ״̬��
            /// </summary>
            /// <param name="activity"></param>
            public static void SetColorStatusBar(Activity activity)
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
            public static void SetKKStatusBar(Activity activity, int statusBarColor)
            {
                SetTransparentStausBar(activity);//��͸����("ȥ��"״̬��)
                ViewGroup contentView = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
                _statusBarView = contentView.GetChildAt(0);
                //��ֹ�ظ����statusBarView
                if (_statusBarView != null && _statusBarView.MeasuredHeight == GetStatusBarHeight(activity))
                {
                    _statusBarView.SetBackgroundColor(activity.Resources.GetColor(statusBarColor));
                    return;
                }
                _statusBarView = new View(activity);
                ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, GetStatusBarHeight(activity));
                _statusBarView.SetBackgroundColor(activity.Resources.GetColor(statusBarColor));//���ĵ�״̬����view������ɫ
                contentView.AddView(_statusBarView, lp);
            }
            private static int GetStatusBarHeight(Context context)
            {
                int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                return context.Resources.GetDimensionPixelSize(resourceId);
            }
        }
}
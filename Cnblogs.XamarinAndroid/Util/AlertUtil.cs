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
    public class AlertUtil
    {
        private static Toast toast;
        public static void ToastShort(Context context,string  msg)
        {
            if (toast == null)
            {
                toast = Toast.MakeText(context, msg, ToastLength.Short);
            }
            else
            {
                toast.SetText(msg);
            }
            toast.Show();
        }
        public static void ToastLong(Context context, string msg)
        {
            if (toast == null)
            {
                toast = Toast.MakeText(context, msg, ToastLength.Short);
            }
            else
            {
                toast.SetText(msg);
            }
            toast.Show();
        }
    }
}
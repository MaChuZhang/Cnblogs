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
    public static class DateTimeUtil
    {
        public static string ToCommonString(this DateTime dt)
        {
            TimeSpan ts = DateTime.Now.Subtract(dt);
            if (ts.Days > 0)
            {
                int month = (DateTime.Now.Year - dt.Year) * 12 + DateTime.Now.Month - dt.Month;
                if (month >= 12)
                {
                    return $"{month / 12}��ǰ";
                }
                else if (month > 0)
                {
                    return $"{month}��ǰ";
                }
                else
                {
                    return $"{ts.Days}��ǰ";
                }
            }
            else
            {
                if (ts.Hours > 0)
                {
                    return $"{ts.Hours}Сʱǰ";
                }
                else
                {
                    if (ts.Minutes > 0)
                    {
                        return $"{ts.Minutes}����ǰ";
                    }
                    if (ts.Seconds > 30)
                    {
                        return $"{ts.Seconds}��ǰ";
                    }
                    else
                    {
                        return "�ո�";
                    }
                }
            }
        }
    }
}
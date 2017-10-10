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
                    return $"{month / 12}年前";
                }
                else if (month > 0)
                {
                    return $"{month}月前";
                }
                else
                {
                    return $"{ts.Days}天前";
                }
            }
            else
            {
                if (ts.Hours > 0)
                {
                    return $"{ts.Hours}小时前";
                }
                else
                {
                    if (ts.Minutes > 0)
                    {
                        return $"{ts.Minutes}分钟前";
                    }
                    if (ts.Seconds > 30)
                    {
                        return $"{ts.Seconds}秒前";
                    }
                    else
                    {
                        return "刚刚";
                    }
                }
            }
        }
    }
}
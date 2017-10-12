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
    public static class HtmlUtil
    {
        public static string ReplaceHtml(this string  body)
        {
            if (body != null)
            {
                body = body.Replace("\\r\\n", @"").Replace("\\n", @"").Replace("\\t", "").Replace("\\", "");
                return body;
            }
            return "";
        }
    }
}
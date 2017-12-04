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
using System.IO;
using Android.Text;

namespace Cnblogs.XamarinAndroid
{
    public static class HtmlUtil
    {
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        public static ISpanned GetHtml(string html, FromHtmlOptions flags = FromHtmlOptions.ModeLegacy)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                return Html.FromHtml(html, flags);
            }
            else
            {
                return Html.FromHtml(html);
            }
        }
        public static string ReplaceHtml(this string  body)
        {
            if (string.IsNullOrEmpty(body))
                return "";
            return body.Replace("\\r\\n", @"
 ").Replace("\\n", @"
 ").Replace("\\t", "&nbsp;&nbsp;").Replace("\\u0004", "").Replace("\\", "");
        }
        public static string ReadHtml(Android.Content.Res.AssetManager assets)
        {
            string content = string.Empty;
            try
            {
                using (var stream = assets.Open("content.html"))
                {
                    StreamReader sr = new StreamReader(stream);
                    content = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    return content;
                }
            }
            catch (Exception ex)
            {
                //todo 异常处理
                return content;
            }
        }
    }
}
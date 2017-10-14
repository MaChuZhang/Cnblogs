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

namespace Cnblogs.XamarinAndroid
{
    public static class HtmlUtil
    {
        public static string ReplaceHtml(this string  body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                body = body.Replace("\\r\\n",@"").Replace("\\n",@"").Replace("\\r","").Replace("\\t", "&nbsp;&nbsp;").Replace("\\u0004","").Replace(@"\","");
                return body;
            }
            return "";
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
                //todo “Ï≥£¥¶¿Ì
                return content;
            }
        }
    }
}
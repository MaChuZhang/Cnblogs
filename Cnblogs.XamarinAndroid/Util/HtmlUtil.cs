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
using System.Text.RegularExpressions;

namespace Cnblogs.XamarinAndroid
{
    public static class HtmlUtil
    {
        /// <summary>
        /// 替换strong标签为font标签红色字体
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string replaceStrongToFont(this string htmlStr)
        {
            int hasCount = Regex.Matches(htmlStr, @"<strong>").Count;
            string tempStr = htmlStr;
            while (hasCount >= 0)
            {
                tempStr = htmlStr.Replace("<strong>", "<font color='#ff0000'>").Replace("</strong>", "</font>");
                htmlStr = tempStr;
                hasCount--;
            }
            return tempStr;
        }
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
          
            /// <summary>   
    /// 取得HTML中所有图片的 URL。   
    /// </summary>   
    /// <param name="sHtmlText">HTML代码</param>   
    /// <returns>图片的URL列表</returns>   
    public static string GetHtmlFirstImgUrl(string sHtmlText)   
    {   
      // 定义正则表达式用来匹配 img 标签   
      Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",   RegexOptions.IgnoreCase);   
        
      // 搜索匹配的字符串   
      MatchCollection matches = regImg.Matches(sHtmlText);   
      int i = 0;   
      string[] sUrlList = new string[matches.Count];   
        
      // 取得匹配项列表   
      foreach (Match match in matches)   
      sUrlList[i++] = match.Groups["imgUrl"].Value;
            if (sUrlList.Length > 0)
            {
                return sUrlList[0];
            }
            return ""; 
    }

        public static string GetScoreName(int score)
        {
            if (score > 100000)
            {
                return "大牛九级";
            }
            if (score > 50000)
            {
                return "牛人八级";
            }
            if (score > 20000)
            {
                return "高人七级";
            }
            if (score > 10000)
            {
                return "专家六级";
            }
            if (score > 5000)
            {
                return "大侠五级";
            }
            if (score > 2000)
            {
                return "老鸟四级";
            }
            if (score > 500)
            {
                return "小虾三级";
            }
            if (score > 200)
            {
                return "初学一级";
            }
            return "初学一级";
        }
    }
}
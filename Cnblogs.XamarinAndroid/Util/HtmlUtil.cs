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
        /// �滻strong��ǩΪfont��ǩ��ɫ����
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
        /// ���תȫ��
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
                //todo �쳣����
                return content;
            }
        }
          
            /// <summary>   
    /// ȡ��HTML������ͼƬ�� URL��   
    /// </summary>   
    /// <param name="sHtmlText">HTML����</param>   
    /// <returns>ͼƬ��URL�б�</returns>   
    public static string GetHtmlFirstImgUrl(string sHtmlText)   
    {   
      // ����������ʽ����ƥ�� img ��ǩ   
      Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",   RegexOptions.IgnoreCase);   
        
      // ����ƥ����ַ���   
      MatchCollection matches = regImg.Matches(sHtmlText);   
      int i = 0;   
      string[] sUrlList = new string[matches.Count];   
        
      // ȡ��ƥ�����б�   
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
                return "��ţ�ż�";
            }
            if (score > 50000)
            {
                return "ţ�˰˼�";
            }
            if (score > 20000)
            {
                return "�����߼�";
            }
            if (score > 10000)
            {
                return "ר������";
            }
            if (score > 5000)
            {
                return "�����弶";
            }
            if (score > 2000)
            {
                return "�����ļ�";
            }
            if (score > 500)
            {
                return "СϺ����";
            }
            if (score > 200)
            {
                return "��ѧһ��";
            }
            return "��ѧһ��";
        }
    }
}
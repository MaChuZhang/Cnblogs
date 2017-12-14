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
using Cnblogs.ApiModel;
using Cnblogs.HttpClient;
using Newtonsoft.Json;

namespace Cnblogs.XamarinAndroid
{
    public class AccessTokenUtil
    {
        private const string fileName = Constact.TagAccessToKen;
        public static void SaveToken(Token token, Context context)
        {

            BaseShared.Instance(context, fileName).SetString(Constact.KeyAccessToken, token.access_token);
            BaseShared.Instance(context, fileName).SetString(Constact.KeyTokenType, token.token_type);
            BaseShared.Instance(context, fileName).SetInt(Constact.KeyExpiresIn, token.expires_in);
            BaseShared.Instance(context, fileName).SetDateTime(Constact.KeyRefreshTime, token.RefreshTime);
        }
        public static Token GetToken(Context context)
        {
            Token token = new Token();
            token.access_token = BaseShared.Instance(context, fileName).GetString(Constact.KeyAccessToken, "");
            token.token_type = BaseShared.Instance(context, fileName).GetString(Constact.KeyTokenType, "");
            token.expires_in = BaseShared.Instance(context, fileName).GetInt(Constact.KeyExpiresIn, 0);
            token.RefreshTime = BaseShared.Instance(context, fileName).GetDateTime(Constact.KeyRefreshTime);
            return token;
        }
    }

    public class UserTokenUtil
    {
        private const string fileName = Constact.TagUserToKen;
        public static void SaveToken(Token token, Context context)
        {

            BaseShared.Instance(context, fileName).SetString(Constact.KeyAccessToken, token.access_token);
            BaseShared.Instance(context, fileName).SetString(Constact.KeyTokenType, token.token_type);
            BaseShared.Instance(context, fileName).SetString(Constact.KeyRefreshToken, token.refresh_token);
            BaseShared.Instance(context, fileName).SetInt(Constact.KeyExpiresIn, token.expires_in);
            BaseShared.Instance(context, fileName).SetDateTime(Constact.KeyRefreshTime,DateTime.Now);
            BaseShared.Instance(context, fileName).SetBool(Constact.KeyIsIdentityUser, token.IsIdentityUser);
        }
        public static Token GetToken(Context context)
        {
            Token token = new Token();
            token.access_token = BaseShared.Instance(context, fileName).GetString(Constact.KeyAccessToken, "");
            token.refresh_token = BaseShared.Instance(context, fileName).GetString(Constact.KeyRefreshToken, "");
            token.token_type = BaseShared.Instance(context, fileName).GetString(Constact.KeyTokenType, "");
            token.expires_in = BaseShared.Instance(context, fileName).GetInt(Constact.KeyExpiresIn, 0);
            token.RefreshTime = BaseShared.Instance(context, fileName).GetDateTime(Constact.KeyRefreshTime);
            token.IsIdentityUser = BaseShared.Instance(context, fileName).GetBool(Constact.KeyIsIdentityUser, false);
            return token;
        }
    }

    public class UserInfoShared
    {
        private const string fileName = "USERINFO";
        public static void SetUserInfo(UserInfo  userInfo, Context context)
        {

            BaseShared.Instance(context, fileName).SetString("BlogApp",  userInfo.BlogApp);
            BaseShared.Instance(context, fileName).SetString("Seniority", userInfo.Seniority);
            BaseShared.Instance(context, fileName).SetString("DisplayName", userInfo.DisplayName);
            BaseShared.Instance(context, fileName).SetString("Avatar", userInfo.Avatar);
            BaseShared.Instance(context, fileName).SetString("Avatar", userInfo.Face);
            BaseShared.Instance(context, fileName).SetString("UserId", userInfo.UserId.ToString());
            BaseShared.Instance(context, fileName).SetInt("SpaceUserId", userInfo.SpaceUserId);
            BaseShared.Instance(context, fileName).SetInt("BlogId", userInfo.BlogId);
        }
        public static UserInfo GetUserInfo(Context context)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.BlogApp = BaseShared.Instance(context, fileName).GetString("BlogApp", "");
            userInfo.Seniority = BaseShared.Instance(context, fileName).GetString("Seniority", "");
            userInfo.Avatar = BaseShared.Instance(context, fileName).GetString("Avatar", "");
            userInfo.DisplayName = BaseShared.Instance(context, fileName).GetString("DisplayName", "");
            userInfo.Face = BaseShared.Instance(context, fileName).GetString("Face","");
            //userInfo.UserId = Guid.Parse(BaseShared.Instance(context, fileName).GetString("SetString", ""));
            userInfo.SpaceUserId = BaseShared.Instance(context, fileName).GetInt("SpaceUserId",0);
            userInfo.BlogId = BaseShared.Instance(context, fileName).GetInt("BlogId", 0);
            return userInfo;
        }
    }

    public class UserBlogShared
    {
        private const string fileName = "USERBLOG";
        public static void SetUserBlog(UserBlog userBlog, Context context)
        {

            BaseShared.Instance(context, fileName).SetInt("BlogId", userBlog.BlogId);
            BaseShared.Instance(context, fileName).SetString("Title", userBlog.Title);
            BaseShared.Instance(context, fileName).SetString("SubTitle", userBlog.SubTitle);
            BaseShared.Instance(context, fileName).SetInt("PostCount", userBlog.PostCount);
            BaseShared.Instance(context, fileName).SetInt("PageSize", userBlog.PageSize);
            BaseShared.Instance(context, fileName).SetBool("EnableScript", userBlog.EnableScript);
        }
        public static UserBlog GetUserBlog(Context context)
        {
            UserBlog userInfo = new UserBlog();
            userInfo.BlogId = BaseShared.Instance(context, fileName).GetInt("BlogId", 0);
            userInfo.Title = BaseShared.Instance(context, fileName).GetString("Title", "");
            userInfo.SubTitle = BaseShared.Instance(context, fileName).GetString("SubTitle", "");
            userInfo.PostCount = BaseShared.Instance(context, fileName).GetInt("PostCount", 0);
            userInfo.PageSize = BaseShared.Instance(context, fileName).GetInt("PageSize",10);
            userInfo.EnableScript = BaseShared.Instance(context, fileName).GetBool("EnableScript",false);
            return userInfo;
        }
    }

    public class SearchHistoryShared
    {
        private const string fileName = "SearchHistory";
        public static void SetSearchHistory(string  keyword, Context context)
        {
            var dict = GetSearchHistory(context);
            if (dict == null)
                dict = new Dictionary<string, DateTime>();
            if (dict.ContainsKey(keyword))
                return;
            if (dict.Count == 10)
            {
                var minKeyword =  dict.Select(s => s.Value).Min();
                var removeKey = dict.First(f => f.Value == minKeyword).Key;
                dict.Remove(removeKey);
            }
                dict.Add(keyword, DateTime.Now);
                string str = JsonConvert.SerializeObject(dict);
                BaseShared.Instance(context, fileName).SetString(fileName,str);
        }
        public static Dictionary<string,DateTime> GetSearchHistory(Context context)
        {
            try
            {
                string str = BaseShared.Instance(context, fileName).GetString(fileName, "");
                Dictionary<string, DateTime> dict = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(str);
                dict = dict.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                return dict;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return null;
            }
        }
        public static bool  DeleteSearchHistory(string keyword, Context context)
        {
            try
            {
                var dict = GetSearchHistory(context);
                if (dict == null || dict.Count == 0 || !dict.ContainsKey(keyword))
                    return false;
                dict.Remove(keyword);
                string str = JsonConvert.SerializeObject(dict);
                BaseShared.Instance(context, fileName).SetString(fileName, str);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return false;
            }
        }
        public static bool DeleteSearchHistory(Context context)
        {
            try
            {
                BaseShared.Instance(context, fileName).SetString(fileName, "");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return false;
            }
        }
    }
}
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

namespace Cnblogs.XamarinAndroid
{
    public class AccessTokenUtil
    {
        private const string fileName = Constact.TagAccessToKen;
        public static void SaveToken(Token token,Context context)
        {
            
            BaseShared.Instance(context, fileName).SetString(Constact.KeyAccessToken,token.access_token);
            BaseShared.Instance(context, fileName).SetString(Constact.KeyTokenType, token.token_type);
            BaseShared.Instance(context, fileName).SetInt(Constact.KeyExpiresIn,token.expires_in);
            BaseShared.Instance(context, fileName).SetDateTime(Constact.KeyRefreshTime, token.RefreshTime);
        }
        public static Token GetToken(Context context)
        {
            Token token = new Token();
            token.access_token = BaseShared.Instance(context, Constact.TagAccessToKen).GetString(Constact.KeyAccessToken, "");
            token.token_type =BaseShared.Instance(context, Constact.TagAccessToKen).GetString(Constact.KeyTokenType, "");
            token.expires_in =BaseShared.Instance(context, Constact.TagAccessToKen).GetInt(Constact.KeyExpiresIn, 0);
            token.RefreshTime =BaseShared.Instance(context, Constact.TagAccessToKen).GetDateTime(Constact.KeyRefreshTime);
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
            BaseShared.Instance(context, fileName).SetInt(Constact.KeyExpiresIn, token.expires_in);
            BaseShared.Instance(context, fileName).SetDateTime(Constact.KeyRefreshTime, token.RefreshTime);
            BaseShared.Instance(context, fileName).SetBool(Constact.KeyIsIdentityUser,token.IsIdentityUser);
        }
        public static Token GetToken(Context context)
        {
            Token token = new Token();
            token.access_token = BaseShared.Instance(context, Constact.TagAccessToKen).GetString(Constact.KeyAccessToken, "");
            token.token_type = BaseShared.Instance(context, Constact.TagAccessToKen).GetString(Constact.KeyTokenType, "");
            token.expires_in = BaseShared.Instance(context, Constact.TagAccessToKen).GetInt(Constact.KeyExpiresIn, 0);
            token.RefreshTime = BaseShared.Instance(context, Constact.TagAccessToKen).GetDateTime(Constact.KeyRefreshTime);
            token.IsIdentityUser = BaseShared.Instance(context, Constact.TagAccessToKen).GetBool(Constact.KeyIsIdentityUser,false);
            return token;
        }
    }
}
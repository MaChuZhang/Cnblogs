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

namespace Cnblogs.XamarinAndroid.Util
{
    public class TokenUtil
    {
        public static Token GetToken(Context context)
        {
            Token token = new Token();
            token.access_token = SharedDataUtil.Instance(context, Constact.Tag_AccessKen).GetString(Constact.KeyAccessToken,"");
            token.token_type = SharedDataUtil.Instance(context, Constact.Tag_AccessKen).GetString(Constact.KeyTokenType, "");
            token.expires_in = SharedDataUtil.Instance(context, Constact.Tag_AccessKen).GetInt(Constact.KeyExpiresIn, 0);
            token.RefreshTime = SharedDataUtil.Instance(context, Constact.Tag_AccessKen).GetDateTime(Constact.KeyRefreshTime);
            return token;
        }
        public static void SetToken(Context context,Token token)
        {
            SharedDataUtil.Instance(context, Constact.Tag_AccessKen).SetString(Constact.KeyAccessToken,token.access_token);
            SharedDataUtil.Instance(context, Constact.Tag_AccessKen).SetString(Constact.KeyTokenType, token.token_type);
            SharedDataUtil.Instance(context, Constact.Tag_AccessKen).SetInt(Constact.KeyExpiresIn, token.expires_in);
            SharedDataUtil.Instance(context, Constact.Tag_AccessKen).SetDateTime(Constact.KeyRefreshTime,token.RefreshTime);
        }
    }
}
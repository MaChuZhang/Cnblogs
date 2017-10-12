using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cnblogs.HttpClient
{
    public class Constact
    {
        public const string Tag_AccessKen = "AccessToken";
        public const string KeyAccessToken = "access_token";
        public const string KeyTokenType = "token_type";
        public const string KeyExpiresIn = "expires_in";
        public const string KeyIsIdentityUser = "IsIdentityUser";
        public const string KeyRefreshTime = "RefreshTime";

        public const string client_id_firend = "c36f3da8-81fa-4466-9bd7-554da9562a4c";
        public const string client_secret_firend = "kFLGBuwFNdPLLU8jw3DdCVFOmGz6nN5ZvI1yd-inFK8qRQze2EzsW63Vh7B1fzXCKsZgQx-q3eM7zU6b";
        public const  string client_id = "22f870a0-0e44-4439-bbb1-2cde1b5339c6";
        public const string client_secret = "Xdc4V-jrnPPfJtRi3d1amMM93FAgYup6nJcVZwKFPU6vxRDX21znCFamWCuwCUfs5gv8XpCEeVXFAnp6";
        public const string Host = "https://api.cnblogs.com";
        public const string Api = "/api";

        public const string grant_type = "password";
        public const string code = ""; //授权码
        public const string redirect_uri = "https://oauth.cnblos.com/auth/callback"; //回调地址
        public const  string Content_Type = "application/x-www-form-urlencoded";

        public const string ConnectToken = "https://oauth.cnblogs.com/connect/token";
        public const string  GetAuthrize = "https://oauth.cnblogs.com/connect/authorize?client_id=22f870a0-0e44-4439-bbb1-2cde1b5339c6&scope=openid profile CnBlogsApi offline_access&response_type=code id_token&redirect_uri=https://oauth.cnblogs.com/auth/callback&state=cnblogs.com&nonce=zhanglin";
        public const string Callback = "https://oauth.cnblogs.com/auth/callback";
        public const string test = "https://oauth.cnblogs.com/connect/authorize?client_id=c36f3da8-81fa-4466-9bd7-554da9562a4c&scope=openid profile CnBlogsApi offline_access&response_type=code id_token&redirect_uri=https://oauth.cnblogs.com/auth/callback&state=cnblogs.com&nonce=zhanglin";

        public const string SiteHomeArticleList = Host+Api+"/blogposts/@sitehome?pageIndex={0}&pageSize={1}";
        public const string ArticleBody = Host + Api + "/blogposts/{0}/body";

    }
}
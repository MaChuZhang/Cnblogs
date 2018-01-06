using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cnblogs.HttpClient
{
    public class Constact
    {
        public const string TagAccessToKen = "AccessToken";
        public const string TagUserToKen = "UserToken";
        public const string KeyAccessToken = "access_token";
        public const string KeyTokenType = "token_type";
        public const string KeyExpiresIn = "expires_in";
        public const string KeyIsIdentityUser = "IsIdentityUser";
        public const string KeyRefreshTime = "RefreshTime";
        public const string KeyRefreshToken = "refresh_token";
         

        public const string client_id_firend = "c36f3da8-81fa-4466-9bd7-554da9562a4c";
        public const string client_secret_firend = "kFLGBuwFNdPLLU8jw3DdCVFOmGz6nN5ZvI1yd-inFK8qRQze2EzsW63Vh7B1fzXCKsZgQx-q3eM7zU6b";
        public const  string client_id = "22f870a0-0e44-4439-bbb1-2cde1b5339c6";
        public const string client_secret = "Xdc4V-jrnPPfJtRi3d1amMM93FAgYup6nJcVZwKFPU6vxRDX21znCFamWCuwCUfs5gv8XpCEeVXFAnp6";
        public const string Host = "https://api.cnblogs.com";
        public const string Api = "/api";
        public const int PageSize = 10;

        public const string grant_type = "password";
        public const string code = ""; //授权码
        public const string redirect_uri = "https://oauth.cnblos.com/auth/callback"; //回调地址
        public const  string Content_Type = "application/x-www-form-urlencoded";
        public const string KbArticleOrigin = "https://kb.cnblogs.com";
        public const string NewsOrigin = "https://news.cnblogs.com";

        public const string ConnectToken = "https://oauth.cnblogs.com/connect/token";
        public const string  GetAuthrize = "https://oauth.cnblogs.com/connect/authorize?client_id={0}&scope=openid profile CnBlogsApi offline_access&response_type=code id_token&redirect_uri=https://oauth.cnblogs.com/auth/callback&state=cnblogs.com&nonce=zhanglin";
        public const string Callback = "https://oauth.cnblogs.com/auth/callback";
        public const string test = "https://oauth.cnblogs.com/connect/authorize?client_id=c36f3da8-81fa-4466-9bd7-554da9562a4c&scope=openid profile CnBlogsApi offline_access&response_type=code id_token&redirect_uri=https://oauth.cnblogs.com/auth/callback&state=cnblogs.com&nonce=zhanglin";

        public const string SiteHomeArticleList = Host+Api+"/blogposts/@sitehome?pageIndex={0}&pageSize={1}";
        public const string ArticleHot = Host + Api + "/blogposts/@picked?pageIndex={0}&pageSize={1}";
        public const string CnblogsPic = "https://pic.cnblogs.com/face/";
        
        public const string NewsList = Host + Api + "/newsitems?pageIndex={0}&pageSize={1}";
        public const string NewsHotList = Host + Api + "/newsitems/@hot?startDate={0}&endDate={1}&pageIndex={2}&pageSize={3}";
        public const string NewsHotWeekList = Host + Api + "/newsitems/@hot-week?pageIndex={0}&pageSize={1}";
        public const string NewsDiggList = Host + Api + "/newsitems/@recommended?pageIndex={0}&pageSize={1}";
        public const string NewsDetail = Host + Api + "/newsitems/{0}/body";
        public const string NewscommentList = Host + Api + "/news/{0}/comments?pageIndex={1}&pageSize={2}";
        public const string NewsCommentAdd = Host + Api + "/news/{0}/comments";
        public const string NewsCommentDelete = Host + Api + "/newscomments/{0}";
        /// <summary>
        /// 获取博客文章评论内容,params1:博客名,博文编号,pageIndex pageSize
        /// </summary>
        public const string ArticlecommentList = Host + Api + "/blogs/{0}/posts/{1}/comments?pageIndex={2}&pageSize={3}";
       /// <summary>
       /// 添加评论
       /// </summary>
        public const string AddArticleComment= Host + Api + "/blogs/{0}/posts/{1}/comments";
        /// <summary>
        /// 获取用户信息
        /// </summary>
        public const string Users = Host + Api + "/users";
        /// <summary>
        /// 根据博客名blogapp，获取个人博客信息
        /// </summary>

        public const string Blogs = Host + Api + "/blogs/{0}";

        /// <summary>
        ///根据url检查收藏是否存在，head请求方式
        /// </summary>
        public const string BookMarks_exists = Host + Api + "/Bookmarks?url={0}";

        /// <summary>
        /// 根据url删除收藏
        /// </summary>
        public const string BookMarks_delete = Host + Api + "/bookmarks/{0}";
        /// <summary>
        /// 根据id修改收藏 patch方式
        /// </summary>
        public const string BookMarks_patch = Host + Api + "/Bookmarks/{0}";

        /// <summary>
        /// 添加收藏,post请求方式
        /// </summary>
        public const string BookMarks_add = Host + Api + "/Bookmarks";


        /// <summary>
        /// 分页获取收藏列表
        /// </summary>
        public const string BookMarks_list = Host + Api + "/Bookmarks?pageIndex={0}&pageSize={0}";




        /// <summary>
        /// 根据blogapp，pageIndex获取个人博客随笔列表
        /// </summary>
        public const string BlogPosts = Host + Api + "/blogs/{0}/posts?pageIndex={1}";
        /// <summary>
        /// 获取博客文章内容
        /// </summary>
        public const string ArticleBody = Host + Api + "/blogposts/{0}/body";
        /// <summary>
        /// 获取知识库列表
        /// </summary>
        public const string KbPage = "https://kb.cnblogs.com/page/{0}/";
        public const string KbArticles = Host + Api + "/KbArticles?pageIndex={0}&pageSize={1}";
        /// <summary>
        ///知识库内容
        /// </summary>
        public const string KbArticlesBody = Host + Api + "/kbarticles/{0}/body";
        /// <summary>
        /// 闪存列表
        /// </summary>
        public const string Statuses = Host + Api + "/statuses/@{0}?pageIndex={1}&pageSize={2}&tag=";
        /// <summary>
        /// 获取闪存评论列表
        /// </summary>
        public const string StatusesComment = Host + Api + "/statuses/{0}/comments";
        /// <summary>
        /// 根据id获取闪存内容
        /// </summary>
        public const string StatusBody = Host + Api + "/statuses/{0}";
        /// <summary>
        /// 发布闪存
        /// </summary>
        public const string StatusAdd = Host + Api + "/statuses";
        /// <summary>
        /// 删除闪存
        /// </summary>
        public const string StatusDelete = Host + Api + "/statuses/{0}";
        /// <summary>
        /// 删除我的评论闪存
        /// </summary>
        public const string StatusDeleteComment = Host + Api + "/statuses/{0}/comments/{1}";

        /// <summary>
        /// 分页获取首页博问列表
        /// </summary>
        public const string Questions = Host + Api + "/questions/@sitehome?pageIndex={0}&pageSize={1}";
        /// <summary>
        /// 根据类型分页获取博问列表
        /// </summary>
        public const string QuestionsType = Host + Api + "/questions/@{0}?pageIndex={1}&pageSize={2}&spaceUserId={3}";

        /// <summary>
        /// 获取根据id博问详情
        /// </summary>
        public const string QuestionDetail = Host + Api + "/questions/{0}";

        /// <summary>
        /// 获取单个回答的评论列表
        /// </summary>
        public const string QuestionAnswerCommentList = Host + Api + "/questions/answers/{0}/comments";
        /// <summary>
        /// 添加问答的回答的评论,参数1 questionID,参数2 用户名
        /// </summary>
        public const string QuestionAddAnswerComment = Host + Api + "/questions/{0}/answers/{1}/comments?loginName={2}";
        /// <summary>
        /// 删除、编辑问答回答的评论
        /// </summary>
        public const string QuestionDeleteAnswerComment = Host + Api + "/questions/{0}/answers/{1}/comments/{2}";

        /// <summary>
        /// 添加问答
        /// </summary>
        public const string QuestionAdd = Host + Api + "/questions";
        /// <summary>
        ///获取单个问题对应的回答列表
        /// </summary>

        public const string QuestionAnswerList = Host + Api + "/questions/{0}/answers";
        /// <summary>
        /// 添加问答的回答
        /// </summary>
        public const string QuestionAddAnswer = Host + Api + "/questions/{0}/answers?loginName={1}";

        /// <summary>
        /// 删除，修改问答的回答
        /// </summary>
        public const string QuestionDeleteAnswer = Host + Api + "/questions/{0}/answers/{1}";

        public const string Zzk = Host + Api + "/ZzkDocuments/{0}?keyWords={1}&pageIndex={2}";
    }
}

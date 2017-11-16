using Cnblogs.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.HttpClient
{
    public class UserRequest
    {
        public static async Task<ApiResult<UserInfo>> UserInfo(Token token)
        {
            //var result=null;
            try
            {
                var result = await HttpBase.GetAsync(Constact.Users, null, token);
                if (result.IsSuccess)
                {
                    //errorAction("网络请求失败:" + response.StatusCode);
                    var userinfo = JsonConvert.DeserializeObject<UserInfo>(result.Message);
                    return ApiResult.Ok(userinfo);
                }
                return ApiResult<UserInfo>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<UserInfo>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<UserBlog>> UserBlog(Token token,string blogApp)
        {
            //var result=null;
            try
            {
                string url = string.Format(Constact.Blogs, blogApp);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    //errorAction("网络请求失败:" + response.StatusCode);
                    var userinfo = JsonConvert.DeserializeObject<UserBlog>(result.Message);
                    return ApiResult.Ok(userinfo);
                }
                return ApiResult<UserBlog>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<UserBlog>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<List<Article>>> BlogPosts(Token token, string blogApp,int pageIndex)
        {
            //var result=null;
            try
            {
                string url = string.Format(Constact.BlogPosts, blogApp,pageIndex);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<Article>>(result.Message);
                    return   ApiResult.Ok(list);
                }
                return ApiResult<List<Article>>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<List<Article>>.Error(ex.Message);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Cnblogs.ApiModel;
using Cnblogs.HttpClient;

namespace Cnblogs.HttpClient
{
    public class KbArticlesRequest
    {
        public static async Task<ApiResult<List<KbArticles>>> GetKbArticlesList(Token token,int pageIndex)
        {
            try
            {
                string url =string.Format(Constact.KbArticles, pageIndex, 10);
                var result=await HttpBase.GetAsync(url,null,token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<KbArticles>>(result.Message);
                    //successAction(list);
                    return  ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<KbArticles>>.Error(result.Message);
                   //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<KbArticles>>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<string>> GetKbArticlesDetail(Token token, int id)
        {
            try
            {
                string url = string.Format(Constact.KbArticlesBody, id);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var articleDetail = result.Message;
                    //successAction(list);
                    return ApiResult.Ok(articleDetail);
                }
                else
                {
                    return ApiResult<string>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<string>.Error(ex.Message);
            }
        }
    }
}

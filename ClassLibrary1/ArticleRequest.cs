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
    public class ArticleRequest
    {
        public static async Task<ApiResult<List<Article>>> GetArticleList(Token token,int pageIndex,int position)
        {
            try
            {
                string url = string.Empty;
                if (position == 0)
                {
                    url = string.Format(Constact.SiteHomeArticleList, pageIndex, 10);
                }
                else
                {
                    url = string.Format(Constact.ArticleHot, pageIndex, 10);
                }
                var result=await HttpBase.GetAsync(url,null,token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<Article>>(result.Message);
                    //successAction(list);
                    return  ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<Article>>.Error(result.Message);
                   //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<Article>>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<string>> GetArticleDetail(Token token,int id)
        {
            try
            {
                string url = string.Format(Constact.ArticleBody,id);
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
        public static async Task<ApiResult<List<ArticleCommentModel>>> ListArticleComment(Token token,string  blogApp,int postId, int pageIndex)
        {
            try
            {
                string url = string.Format(Constact.ArticleCommentList,blogApp,postId, pageIndex, Constact.PageSize);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<ArticleCommentModel>>(result.Message);
                    //successAction(list);
                    return ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<ArticleCommentModel>>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<ArticleCommentModel>>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<string>> Add(Token token,string  blogApp,int postId,string  body)
        {
            //var result=null;
            try
            {
                string url = string.Format(Constact.AddArticleComment,blogApp,postId);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("body",body);
                var result = await HttpBase.PostAsync(token, url,dict);
                if (result.IsSuccess)
                {
                    return ApiResult.Ok(result.Message);
                    //return   ApiResult.Ok(list);
                }
                return ApiResult<string>.Error(result.Message);
                //return ApiResult<List<Article>>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<string>.Error(ex.Message);
            }
        }
    }
}

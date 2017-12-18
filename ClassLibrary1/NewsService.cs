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
    public class NewsService
    {
        public static async Task<ApiResult<List<NewsViewModel>>> ListNews(Token token,int pageIndex,int position)
        {
            try
            {
                string url = string.Empty;
                switch (position)
                {
                    case 0:
                        url = string.Format(Constact.NewsList, pageIndex, Constact.PageSize);
                        break;
                    case 1:
                        url = string.Format(Constact.NewsHotWeekList, pageIndex, Constact.PageSize);
                        break;
                    case 2:
                        url = string.Format(Constact.NewsDiggList, pageIndex, Constact.PageSize);
                        break;
                    default:
                        url = string.Format(Constact.NewsList, pageIndex, Constact.PageSize);
                        break;
                }
                var result=await HttpBase.GetAsync(url,null,token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<NewsViewModel>>(result.Message);
                    //successAction(list);
                    return  ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<NewsViewModel>>.Error(result.Message);
                   //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<NewsViewModel>>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<string>> GetNewsDetail(Token token,int id)
        {
            try
            {
                string url = string.Format(Constact.NewsDetail,id);
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
        public static async Task<ApiResult<List<NewsCommentViewModel>>> ListNewsComment(Token token,int postId, int pageIndex)
        {
            try
            {
                string url = string.Format(Constact.NewscommentList,postId, pageIndex, Constact.PageSize);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<NewsCommentViewModel>>(result.Message);
                    //successAction(list);
                    return ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<NewsCommentViewModel>>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<NewsCommentViewModel>>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<string>> Add(Token token,int parentId,int postId,string content)
        {
            try
            {
                string url = string.Format(Constact.NewsCommentAdd,postId);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("Content",content);
                dict.Add("ParentId", parentId.ToString());
                var result = await HttpBase.PostAsyncJson(token, url,dict);
                if (result.IsSuccess)
                {
                    return ApiResult.Ok(result.Message);
                }
                return ApiResult<string>.Error(result.Message);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Error(ex.Message);
            }
        }
    }
}

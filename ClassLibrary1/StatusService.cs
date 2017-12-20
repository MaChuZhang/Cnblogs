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
    public class StatusService
    {
        public static async Task<ApiResult<List<StatusModel>>> ListStatus(Token token,int statusType,int pageIndex,bool isMy)
        {
            try
            {
                string _statusType =string.Empty;
                if (isMy)
                {
                    switch (statusType)
                    {
                        case (int)MyStatusType.my:
                            _statusType = "my";
                            break;
                        case (int)MyStatusType.mention:
                            _statusType = "mention";
                            break;
                        case (int)MyStatusType.comment:
                            _statusType = "comment";
                            break;
                        case (int)MyStatusType.mycomment:
                            _statusType = "mycomment";
                            break;
                    }
                }
                else
                {
                    switch (statusType)
                    {
                        case (int)StatusType.all:
                            _statusType = "all";
                            break;
                        case (int)StatusType.recentcomment:
                            _statusType = "recentcomment";
                            break;
                        case (int)StatusType.following:
                            _statusType = "following";
                            break;
                        default:
                            _statusType = "all";
                            break;
                    }
                }
                string url = string.Format(Constact.Statuses,_statusType,pageIndex,Constact.PageSize);
                var result=await HttpBase.GetAsync(url,null,token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<StatusModel>>(result.Message);
                    //successAction(list);
                    return  ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<StatusModel>>.Error(result.Message);
                   //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<StatusModel>>.Error(ex.Message);
            }
        }

        public static async Task ListStatusComment(Token token,int id,Action<List<StatusCommentsModel>> callBackSuccess,Action<string> callBackError)
        {
            try {
                string url = string.Format(Constact.StatusesComment, id);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<StatusCommentsModel>>(result.Message);
                    //successAction(list);
                    callBackSuccess(list);
                }
                else
                {
                    callBackError(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                callBackError(ex.ToString());
            }
        }
   

        public static async Task<ApiResult<bool>> Add(Token token,string content,bool  isPrivate)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Content",content);
            dict.Add("IsPrivate",isPrivate.ToString());
            var result =await HttpBase.PostAsyncJson(token,Constact.StatusAdd,dict);
            if (result.IsSuccess)
            {
                return  ApiResult.Ok(true);
            }
            else return ApiResult<bool>.Error(result.Message);
        }

        public static void Delete(Token token,int id,Action<string> callBackSuccess,Action<string> callBackError)
        {
            string url = string.Format(Constact.StatusDelete, id);
            HttpBase.Delete(url,token,(response)=> {
                if (response.IsSuccess)
                {
                    callBackSuccess(response.Message);
                }
                else
                {
                    callBackError(response.Message);
                }
            });
        }
        public static void DeleteComment(Token token ,string  statusId ,string id , Action callBackSuccess,Action<string> callBackError)
        {
            string url = string.Format(Constact.StatusDeleteComment,statusId,id);
            HttpBase.Delete(url,token,(response)=> {
                if (response.IsSuccess)
                {
                    callBackSuccess();
                }
                else
                {
                    callBackError(response.Message);
                }
            });
        }
        /// <summary>
        /// 添加闪存评论
        /// </summary>
        /// <param name="token"></param>
        /// <param name="statusId"></param>
        /// <param name="replyTo"></param>
        /// <param name="parentCommentId"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public static async Task<ApiResult<bool>> AddComment(Token token, string statusId, int replyTo,  int parentCommentId, string Content)
        {
            string url = string.Format(Constact.StatusesComment, statusId);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("ReplyTo", replyTo.ToString());
            dict.Add("ParentCommentId", parentCommentId.ToString());
            dict.Add("Content", Content);
            var result =  await HttpBase.PostAsyncJson(token, url, dict);
            System.Diagnostics.Debug.Write("PostAsyncJson", "PostAsyncJson");
            if (result.IsSuccess)
            {
                return ApiResult.Ok(true);
            }
            else
            {
                return ApiResult<bool>.Error(result.Message);
            }
        }
    }
}

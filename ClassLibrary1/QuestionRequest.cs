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
    public enum QuestionType
    {
         unsolved,highscore,noanswer,solved
    }
    public enum MyQuestionType
    {
        myquestion, myunsolved, myanswer, mybestanswer
    }
    public class QuestionRequest
    {
        /// <summary>
        /// 分类获取问答
        /// </summary>
        /// <param name="token"></param>
        /// <param name="questionType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isMy"></param>
        /// <param name="spaceUserId"></param>
        /// <returns></returns>
        public static async Task<ApiResult<List<QuestionModel>>> ListQuestion(Token token, int questionType, int pageIndex, bool isMy, int spaceUserId)
        {
            try
            {
                string _questionType = string.Empty;
                if (isMy)
                {
                    switch (questionType)
                    {
                        case (int)MyQuestionType.myquestion:
                            _questionType = "myquestion";
                            break;
                        case (int)MyQuestionType.myunsolved:
                            _questionType = "myunsolved";
                            break;
                        case (int)MyQuestionType.myanswer:
                            _questionType = "myanswer";
                            break;
                        case (int)MyQuestionType.mybestanswer:
                            _questionType = "mybestanswer";
                            break;
                        default:
                            _questionType = "myquestion";
                            break;
                    }
                }
                else
                {
                    switch (questionType)
                    {
                        case (int)QuestionType.unsolved:
                            _questionType = "unsolved";
                            break;
                        case (int)QuestionType.highscore:
                            _questionType = "highscore";
                            break;
                        case (int)QuestionType.noanswer:
                            _questionType = "noanswer";
                            break;
                        case (int)QuestionType.solved:
                            _questionType = "solved";
                            break;
                        default:
                            _questionType = "unsolved";
                            break;
                    }
                }
                string url = string.Format(Constact.QuestionsType, _questionType, pageIndex, Constact.PageSize, spaceUserId);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<QuestionModel>>(result.Message);
                    //successAction(list);
                    return ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<QuestionModel>>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<QuestionModel>>.Error(ex.Message);
            }
        }


        /// <summary>
        ///根据id获取问答详情
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ApiResult<QuestionModel>> GetQuestionDetail(Token token, int id)
        {
            try
            {
                string url = string.Format(Constact.QuestionDetail, id);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var articleDetail = JsonConvert.DeserializeObject<QuestionModel>(result.Message);
                    //successAction(list);
                    return ApiResult.Ok(articleDetail);
                }
                else
                {
                    return ApiResult<QuestionModel>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<QuestionModel>.Error(ex.Message);
            }
        }
        /// <summary>
        /// 添加问答
        /// </summary>
        /// <param name="token"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="tags"></param>
        /// <param name="flags"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<ApiResult<bool>> Add(Token token, string title, string content, string tags, string flags, string userId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Title", title);
            dict.Add("Content", content);
            dict.Add("Tags", tags);
            dict.Add("Flags", flags);
            dict.Add("UserID", userId);
            var result = await HttpBase.PostAsyncJson(token, Constact.QuestionAdd, dict);
            if (result.IsSuccess)
            {
                return ApiResult.Ok(true);
            }
            else
            {
                return ApiResult<bool>.Error(result.Message);
            }
        }
        /// <summary>
        /// 获取单个回答的评论列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public static async Task<ApiResult<List<QuestionCommentViewModel>>> ListQuestionAnswerComment(Token token, int answerId)
        {
            string url = string.Format(Constact.QuestionAnswerCommentList, answerId);
            var result = await HttpBase.GetAsync(url, null, token);
            if (result.IsSuccess)
            {
                var list = JsonConvert.DeserializeObject<List<QuestionCommentViewModel>>(result.Message);
                return ApiResult.Ok(list);
            }
            else
                return ApiResult<List<QuestionCommentViewModel>>.Error(result.Message);
        }
        /// <summary>
        /// 添加回答的评论
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <param name="loginName">提交者用户名</param>
        /// <param name="questionId">问题ID</param>
        /// <param name="answerId">回答ID</param>
        /// <param name="parentCommentId">回答的父ID</param>
        /// <param name="postUserId">提交者ID</param>
        /// <returns></returns>
        public static async Task<ApiResult<bool>> AddQuestionAnswerComment(Token token,string content,string loginName,int questionId,int answerId,int parentCommentId,int postUserId)
        {
            string  url =string.Format(Constact.QuestionAddAnswerComment,questionId, answerId,loginName);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Content",content);
            dict.Add("ParentCommentId", parentCommentId.ToString());
            dict.Add("PostUserID",postUserId.ToString());
            var result = await HttpBase.PostAsyncJson(token, url, dict);
            if (result.IsSuccess)
            {
                return ApiResult.Ok(true);
            }
            else
            {
                return ApiResult<bool>.Error(result.Message);
            }
        }

        /// <summary>
        /// 修改回答的评论
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <param name="loginName">提交者用户名</param>
        /// <param name="questionId">问题ID</param>
        /// <param name="answerId">回答ID</param>
        /// <param name="parentCommentId">回答的父ID</param>
        /// <param name="postUserId">提交者ID</param>
        /// <returns></returns>
        public static void EditQuestionAnswerComment(Token token, string content, int questionId, int answerId, int commentId, int postUserId,Action successCallBack,Action<string> errorCallBack)
        {
            string url = string.Format(Constact.QuestionDeleteAnswerComment, questionId, answerId, commentId);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Content", content);
            //dict.Add("questionId", questionId.ToString());
            //dict.Add("answerId", answerId.ToString());
            //dict.Add("commentId", commentId.ToString());
            dict.Add("PostUserID",postUserId.ToString());
             HttpBase.Patch(url,token, dict,(response)=> {
                 if (response.IsSuccess)
                 {
                     successCallBack();
                 }
                 else
                 {
                     errorCallBack(response.Message);
                 }
            });
        }


        /// <summary>
        /// 删除回答的评论
        /// </summary>
        /// <returns></returns>
        public static void DeleteQuestionAnswerComment(Token token, int questionId, int answerId,int commentId, Action<string> callBackError, Action callBackSuccess)
        {
            string url = string.Format(Constact.QuestionDeleteAnswerComment, questionId, answerId,commentId);
            HttpBase.Delete(url, token, (response) => {
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
        /// 获取单个问答的回答列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public static async Task<ApiResult<List<QuestionAnswerViewModel>>> ListQuestionAnswer(Token token, int answerId)
        {
            string url = string.Format(Constact.QuestionAnswerList, answerId);
            var result = await HttpBase.GetAsync(url, null, token);
            if (result.IsSuccess)
            {
                var list = JsonConvert.DeserializeObject<List<QuestionAnswerViewModel>>(result.Message);
                return ApiResult.Ok(list);
            }
            else
                return ApiResult<List<QuestionAnswerViewModel>>.Error(result.Message);
        }

        /// <summary>
        /// 添加问答的回答
        /// </summary>
        public static async Task<ApiResult<bool>> AddQuestionAnswer(Token token , string loginName, string UserName, string answerContent, int questionId,int userId )
        {
            string url = string.Format(Constact.QuestionAddAnswer,questionId,loginName);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Answer", answerContent);
            dict.Add("UserId", questionId.ToString());
            dict.Add("UserName", UserName);
            var result = await HttpBase.PostAsyncJson(token,url,dict);
            if (result.IsSuccess)
            {
                return ApiResult.Ok(true);
            }
            else
                return ApiResult<bool>.Error(result.Message);
        }

        /// <summary>
        /// 编辑问答的自己回答
        /// </summary>
        /// <param name="token"></param>
        /// <param name="content"></param>
        /// <param name="questionId"></param>
        /// <param name="answerId"></param>
        /// <param name="postUserId"></param>
        /// <param name="successCallBack"></param>
        /// <param name="errorCallBack"></param>
        public static void EditQuestionAnswer(Token token, string content, int questionId, int answerId, int postUserId, Action successCallBack, Action<string> errorCallBack)
        {
            string url = string.Format(Constact.QuestionDeleteAnswer, questionId, answerId);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Answer", content);
            //dict.Add("questionId", questionId.ToString());
            //dict.Add("answerId", answerId.ToString());
            //dict.Add("commentId", commentId.ToString());
            dict.Add("UserID", postUserId.ToString());
            HttpBase.Patch(url, token, dict, (response) => {
                if (response.IsSuccess)
                {
                    successCallBack();
                }
                else
                {
                    errorCallBack(response.Message);
                }
            });
        }
        /// <summary>
        /// 删除问答的回答
        /// </summary>
        /// <returns></returns>
        public static void DeleteQuestionAnswer(Token token ,int questionId,int answerId,Action<string> callBackError,Action callBackSuccess)
        {
            string url = string.Format(Constact.QuestionDeleteAnswer,questionId,answerId);
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

    }
}

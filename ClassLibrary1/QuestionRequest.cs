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
    public class  QuestionRequest
    {
        public static async Task<ApiResult<List<QuestionModel>>> ListQuestion(Token token,int questionType,int pageIndex,bool  isMy)
        {
            try
            {
                string _questionType =string.Empty;
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
                string url = string.Format(Constact.QuestionsType, _questionType, pageIndex,Constact.PageSize);
                var result=await HttpBase.GetAsync(url,null,token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<QuestionModel>>(result.Message);
                    //successAction(list);
                    return  ApiResult.Ok(list);
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

        public static async Task<ApiResult<List<StatusCommentsModel>>> ListStatusComment(Token token,int id)
        {
            try {
                string url = string.Format(Constact.StatusesComment, id);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<StatusCommentsModel>>(result.Message);
                    //successAction(list);
                    return ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<StatusCommentsModel>>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return ApiResult<List<StatusCommentsModel>>.Error(ex.ToString());
            }
        }
        public static async Task<ApiResult<QuestionModel>> GetQuestionDetail(Token token,int id)
        {
            try
            {
                string url = string.Format(Constact.QuestionDetail,id);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var articleDetail =JsonConvert.DeserializeObject<QuestionModel>(result.Message);
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
    }
}

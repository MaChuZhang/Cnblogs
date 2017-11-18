using Cnblogs.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.HttpClient
{
    public class BookmarksRequest
    {
        public static async Task<ApiResult<BookmarksModel>> List(Token token)
        {
            //var result=null;
            try
            {
                var result = await HttpBase.GetAsync(Constact.BookMarks_list, null, token);
                if (result.IsSuccess)
                {
                    //errorAction("网络请求失败:" + response.StatusCode);
                    var userinfo = JsonConvert.DeserializeObject<BookmarksModel>(result.Message);
                    return ApiResult.Ok(userinfo);
                }
                return ApiResult<BookmarksModel>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<BookmarksModel>.Error(ex.Message);
            }
        }


        public static async Task<ApiResult<string>> Add(Token token, BookmarksModel  model)
        {
            //var result=null;
            try
            {
                string url = string.Format(Constact.BookMarks_add);
                var result = await HttpBase.PostAsync(token,url, null);
                if (result.IsSuccess)
                {
                    var  msg = JsonConvert.DeserializeObject<string>(result.Message);
                    return ApiResult.Ok(msg);
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

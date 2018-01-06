using Cnblogs.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.HttpClient
{
    public class BookmarksService
    {
        public static async Task<ApiResult<List<BookmarksModel>>> List(Token token,int pageSize,int pageIndex)
        {
            //var result=null;
            try
            {
                var result = await HttpBase.GetAsync(Constact.BookMarks_list, null, token);
                if (result.IsSuccess)
                {
                    //errorAction("网络请求失败:" + response.StatusCode);
                    var list = JsonConvert.DeserializeObject<List<BookmarksModel>>(result.Message);
                    return ApiResult.Ok(list);
                }
                return ApiResult<List<BookmarksModel>>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<List<BookmarksModel>>.Error(ex.Message);
            }
        }


        public static async Task<ApiResult<string>> Add(Token token, BookmarksModel  model)
        {
            //var result=null;
            try
            {
                string url = string.Format(Constact.BookMarks_add);
                Dictionary<string, string> _params = new Dictionary<string, string>();
                _params.Add("LinkUrl",model.LinkUrl);
                _params.Add("Title",model.Title);
                _params.Add("Summary",model.Summary);
                _params.Add("Tags",model.Tag);
                var result = await HttpBase.PostAsync(token,url, _params);
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


        public static void Delete(Token token,int id,Action<ResponseMessage>  callBack)
        {
                string url = string.Format(Constact.BookMarks_delete, id);
                HttpBase.Delete(url, token, callBack);
        }

        public static void Edit(Token token, BookmarksModel model, Action<ResponseMessage> callBack)
        {
            string url = string.Format(Constact.BookMarks_patch, model.WzLinkId);
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("LinkUrl", model.LinkUrl);
            _params.Add("Title", model.Title);
            _params.Add("Summary", model.Summary);
            _params.Add("Tags", model.Tag);
            _params.Add("WzLinkId",model.WzLinkId.ToString());
            HttpBase.Patch(url, token,_params, callBack);
        }
        public static async Task<ApiResult<bool>> Exist(Token token,string  _url)
        {
            try
            {
                string url = string.Format(Constact.BookMarks_exists,_url);
                var result = await HttpBase.GetAsync( url, null,token);
                if (result.IsSuccess)
                {
                    return ApiResult.Ok(true);
                }
                return ApiResult.Ok(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return ApiResult<bool>.Ok(false);
            }
        }
    }
}

using Cnblogs.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.HttpClient
{
    public class ZzkService
    {
        public static async Task<ApiResult<List<ZzkDocumentViewModel>>> List(Token token,int pageIndex,string category ,string keyword)
        {
            //var result=null;
            try
            {
                string url = string.Format(Constact.Zzk, category, keyword, pageIndex);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    List<ZzkDocumentViewModel> list;
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        //errorAction("网络请求失败:" + response.StatusCode);
                        list= JsonConvert.DeserializeObject<List<ZzkDocumentViewModel>>(result.Message);
                        return ApiResult.Ok(list);
                    }
                  //  return ApiResult.Ok(null);
                }
                return ApiResult<List<ZzkDocumentViewModel>>.Error(result.Message);

            }
            catch (Exception ex)
            {
                return ApiResult<List<ZzkDocumentViewModel>>.Error(ex.Message);
            }
        }
    }
}

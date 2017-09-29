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
        public static async Task GetArticleList(Action<List<Article>> successAction,Action<string>  errorAction)
        {
            try
            {
                string url = "";
                url = string.Format(Constact.SiteHomeArticleList, 1,10);
                var result=await HttpBase.GetAsync(url,null);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<Article>>(result.Message);
                    successAction(list);
                }
                else
                {
                    errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                errorAction(ex.StackTrace.ToString());
            }
        }
    }
}

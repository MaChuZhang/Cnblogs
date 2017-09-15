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
    public class UserRequest
    {
        public static async void Login(string code,Action successAction,Action<string>  errorAction)
        {
            try
            {
                var client = RestClientInstance.Get("https://oauth.cnblogs.com/connect/token");
                //client.BaseUrl = (Uri)url;
                RestRequest request = new RestRequest();
                //request.AddHeader("Content-Type", ConstactUrl.Content_Type);
                request.AddParameter("client_id", ConstactUrl.client_id);
                request.AddParameter("client_secret", ConstactUrl.client_secret);
                request.AddParameter("grant_type", "client_credentials");
                //request.AddParameter("code", code);
                //request.AddParameter("redirect_uri", ConstactUrl.redirect_uri);
                var response = await client.ExecutePostTaskAsync(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    errorAction("网络请求失败:" + response.StatusCode);
                    return;
                }
                if (string.IsNullOrEmpty(response.Content))
                {
                    errorAction("返回数据有误");
                    return;
                }
                var result = JsonConvert.DeserializeObject<Token>(response.Content);
                ModelFactory.token = result;
                successAction(); 
            }
            catch (Exception ex)
            {
                errorAction(ex.StackTrace.ToString());
            }
        }
    }
}

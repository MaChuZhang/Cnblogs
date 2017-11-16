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
    public class AuthorizationRequest
    {
        public static async Task Client_Credentials(Action<Token> successAction,Action<string>  errorAction)
        {
            try
            {
                var client = HttpBase.Instance("https://oauth.cnblogs.com/connect/token");
                RestRequest request = new RestRequest();
                request.AddParameter("client_id", Constact.client_id);
                request.AddParameter("client_secret", Constact.client_secret);
                request.AddParameter("grant_type", "client_credentials");
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
                var token = JsonConvert.DeserializeObject<Token>(response.Content);
                successAction(token); 
            }
            catch (Exception ex)
            {
                errorAction(ex.StackTrace.ToString());
            }
        }

        public static async Task Authorization_Code(Token token,string  code,Action<Token> successAction, Action<string> errorAction)
        {
            try
            {
                RestClient restClient = HttpBase.Instance(Constact.ConnectToken);
                RestRequest request = new RestRequest();
                request.AddParameter("client_id", Constact.client_id);
                request.AddParameter("client_secret", Constact.client_secret);
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", Constact.Callback);
                request.AddHeader("Authorization", token.token_type + " " + token.access_token);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                var response =await restClient.ExecutePostTaskAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Token  userToken = JsonConvert.DeserializeObject<Token>(response.Content);
                    successAction(userToken);
                }
                else
                {
                    errorAction(response.StatusCode+":"+response.Content.ToString());
                }
            }
            catch (Exception ex)
            {
                errorAction(ex.StackTrace.ToString());
            }
        }
    }
}

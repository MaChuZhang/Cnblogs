using Cnblogs.ApiModel;
using Cnblogs.HttpClient;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public class HttpBase
    {
       // public static Token token;
      //  private static RestClient restClient;
        internal static RestClient Instance(string url)
        {
            var restClient = new RestClient(url)
            {
                Timeout = 5000,
                ReadWriteTimeout = 5000
            };
            return restClient;
        }
        internal static  RestRequest RequestAddHeader(Token token)
        {
            var request = new RestRequest();
            request.AddHeader("Authorization",token.token_type+" "+ ModelFactory.token.access_token);
            return request;
        }

        public static async Task<ResponseMessage> GetAsync(string url,Dictionary<string,string> _params,Token token)
        {
            RestClient restClient = Instance(url);
            RestRequest request = new  RestRequest();
            if (_params != null)
            {
                foreach (var kv in _params)
                {
                    request.AddParameter(kv.Key, kv.Value);
                }
            }

            request.AddHeader("Authorization", token.token_type + " " + token.access_token);
            var response = await restClient.ExecuteGetTaskAsync(request);
            var statusCode = response.StatusCode;
            switch (statusCode)
            {
                case (System.Net.HttpStatusCode.OK):
                    return new ResponseMessage(true,response.Content);
                case System.Net.HttpStatusCode.NotFound:
                    return new ResponseMessage(false,"errorCode:404NotFound");
                case System.Net.HttpStatusCode.Unauthorized:
                    return new ResponseMessage(false,"errorCode:401Unauthorized");
                case System.Net.HttpStatusCode.InternalServerError:
                    return new ResponseMessage(false,"errorCode:500InternalServerError");
                default:
                    return new ResponseMessage(false,"请检查网络连接，稍后再试");
            }
        }
    }
}

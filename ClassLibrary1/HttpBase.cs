using Android.Provider;
using Cnblogs.ApiModel;
using Cnblogs.HttpClient;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            try
            {
                var response = await restClient.ExecuteGetTaskAsync(request);
                var statusCode = response.StatusCode;
                switch (statusCode)
                {
                    case (System.Net.HttpStatusCode.OK):
                        return new ResponseMessage(true, response.Content);
                    case System.Net.HttpStatusCode.NotFound:
                        return new ResponseMessage(false, "errorCode:404NotFound");
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new ResponseMessage(false, "errorCode:401Unauthorized");
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseMessage(false, "errorCode:500InternalServerError");
                    default:
                        return new ResponseMessage(false, "请检查网络连接，稍后再试");
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                return null;
            };
        }
        //private void callBack(RestResponse response, RestRequestAsyncHandle handle)
        //{

        //}
        public static void Delete(string url, Token token,Action<ResponseMessage> callBack)
        {
            RestClient restClient = Instance(url);
            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", token.token_type + " " + token.access_token);
            request.Method = Method.DELETE;
            restClient.DeleteAsync(request, (response, handle) =>
            {
                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode.OK):
                        callBack(new ResponseMessage (true,response.Content));
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        callBack(new ResponseMessage(true, response.Content));
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        callBack(new ResponseMessage(true, response.Content));
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        callBack(new ResponseMessage(true, response.Content));
                        break;
                    default:
                        callBack(new ResponseMessage(true, "请检查网络连接，稍后再试"));
                        break;
                        //return new ResponseMessage(false, "请检查网络连接，稍后再试");
                }
            });
        }

        public static void Patch(string url, Token token, Dictionary<string,string> _params,Action<ResponseMessage> callBack)
        {
            RestClient restClient = Instance(url);
            RestRequest request = new RestRequest();
            if (_params != null)
            {
                foreach (var kv in _params)
                {
                    request.AddParameter(kv.Key, kv.Value);
                }
            }
            request.AddHeader("Authorization", token.token_type + " " + token.access_token);
            request.Method = Method.PATCH;
            restClient.PatchAsync(request, (response, handle) =>
            {
                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode.OK):
                        callBack(new ResponseMessage(true, response.Content));
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        callBack(new ResponseMessage(false, response.Content));
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        callBack(new ResponseMessage(false, response.Content));
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        callBack(new ResponseMessage(false, response.Content));
                        break;
                    default:
                        callBack(new ResponseMessage(false, "请检查网络连接，稍后再试"));
                        break;
                        //return new ResponseMessage(false, "请检查网络连接，稍后再试");
                }
            });
        }
        public static async Task<ResponseMessage> PostAsync(Token token,string url, Dictionary<string, string> _params)
        {
            RestClient restClient = Instance(url);
            RestRequest request = new RestRequest();
            if (_params != null)
            {
                foreach (var kv in _params)
                {
                    request.AddParameter(kv.Key, kv.Value);
                }
            }
            if (token != null)
            {
                request.AddHeader("Authorization", token.token_type + " " + token.access_token);
            }
            //request.AddBody
           request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.Method = Method.POST;
            var response = await restClient.ExecutePostTaskAsync(request);
            var statusCode = response.StatusCode;
            switch (statusCode)
            {
                case (System.Net.HttpStatusCode.Created):
                    return new ResponseMessage(true, "Created");
                case (System.Net.HttpStatusCode.Conflict):
                    return new ResponseMessage(false, "Conflict409,你已经收藏了");
                case System.Net.HttpStatusCode.NotFound:
                    return new ResponseMessage(false, "errorCode:404NotFound");
                case System.Net.HttpStatusCode.Unauthorized:
                    return new ResponseMessage(false, "errorCode:401Unauthorized");
                case System.Net.HttpStatusCode.InternalServerError:
                    return new ResponseMessage(false, "errorCode:500InternalServerError");
                default:
                    return new ResponseMessage(false, "请检查网络连接，稍后再试");
            }
        }
        public static async Task<ResponseMessage> PostAsyncJson(Token token, string url, Dictionary<string, string> _params)
        {
            RestClient restClient = Instance(url);
            RestRequest request = new RestRequest();
            
            if (token != null)
            {
                request.AddHeader("Authorization", token.token_type + " " + token.access_token);
            }
            request.AddHeader("Content-Type", "application/json");
            if (_params != null)
            {
                foreach (var kv in _params)
                {
                    request.AddParameter(kv.Key, kv.Value);
                }
            }
            var response = await restClient.ExecutePostTaskAsync(request);
            var statusCode = response.StatusCode;
            switch (statusCode)
            {
                case (System.Net.HttpStatusCode.Created):
                    return new ResponseMessage(true, "Created");
                case (System.Net.HttpStatusCode.Conflict):
                    return new ResponseMessage(false, "Conflict409");
                case System.Net.HttpStatusCode.NotFound:
                    return new ResponseMessage(false, "errorCode:404NotFound");
                case System.Net.HttpStatusCode.Unauthorized:
                    return new ResponseMessage(false, "errorCode:401Unauthorized");
                case System.Net.HttpStatusCode.InternalServerError:
                    return new ResponseMessage(false, "errorCode:500InternalServerError");
                case HttpStatusCode.UnsupportedMediaType:
                    return new ResponseMessage(false, "errorCode:415UnsupportedMediaType");
                default:
                    return new ResponseMessage(false, "请检查网络连接，稍后再试");
            }
        }
    }
}

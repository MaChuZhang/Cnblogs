using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
namespace Cnblogs.HttpClient
{
    internal class RestClientInstance
    {
        private static RestClient restClient;
        internal static RestClient Get(string url)
        {
            var restClient = new RestClient(url)
            {
                Timeout = 5000,
                ReadWriteTimeout = 5000
            };
            return restClient;        
        }
    }
}

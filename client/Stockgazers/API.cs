using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stockgazers.APIs
{
    public class API
    {
        public static string StockX = "";
        public static string DanielSrv = "";

        public async Task<HttpResponseMessage> Call(HttpClient client, HttpMethod method, string url, string param)
        {
            string urlparam = string.Empty;
            var request = new HttpRequestMessage(method, urlparam);
            return await client.SendAsync(request);
        }
    }
}

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
        static readonly string StockXAccount = "https://accounts.stockx.com/authorize";
        static readonly string DanielSrv = "https://stockgazers.kr";

        static readonly string APIKey = "qEEyuy3uQvFsS1zcxTIB2V8z7PaGcd9nE3MjYW30";
        static readonly string ClientID = "xXybtOs6BRBiJS5aFylrjfHATztZyV64";
        static readonly string ClientSecret = "HL6hPdDNqn5Ectb3iiDDfkcjzH9XmAoDiTPkAd451zjNuUDpFMGaI5MMb3SU5gIT";

        public static string GetAuth()
        {
            return StockXAccount;
        }

        public static string GetID()
        {
            return ClientID;
        }

        public static string GetCallback()
        {
            return DanielSrv;
        }

        public async Task<HttpResponseMessage> Call(HttpClient client, HttpMethod method, string url, string? param=null)
        {
            string urlparam = param == null || param.Length == 0 ? url : url+"?"+param;
            var request = new HttpRequestMessage(method, urlparam);
            return await client.SendAsync(request);
        }
    }
}

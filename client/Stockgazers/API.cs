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
        //static readonly string DanielSrv = "https://stockgazers.kr";
        static readonly string DanielSrv = "http://127.0.0.1:8000";

        static string? strAPIKey = null;
        static string? strClientID = null;
        static string? strClientSecret = null;

        public static string? APIKey
        {
            get {  return strAPIKey; }
            set { strAPIKey = value; }
        }

        public static string? ClientID
        {
            get { return strClientID; }
            set { strClientID = value; }
        }

        public static string? ClientSecret
        {
            get { return strClientSecret; }
            set { strClientSecret = value; }
        }

        public static string GetAuth()
        {
            return StockXAccount;
        }

        public static string GetServer()
        {
            return DanielSrv;
        }

        public static string GetCallback()
        {
            return DanielSrv+"/api/callback";
        }

        public async Task<HttpResponseMessage> Call(HttpClient client, HttpMethod method, string url, string? param=null)
        {
            string urlparam = param == null || param.Length == 0 ? url : url+"?"+param;
            var request = new HttpRequestMessage(method, urlparam);
            return await client.SendAsync(request);
        }
    }
}

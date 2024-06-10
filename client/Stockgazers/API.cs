using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Stockgazers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


#pragma warning disable CS8602
namespace Stockgazers.APIs
{

    public class API
    {
        static readonly string StockXAccount = "https://accounts.stockx.com/authorize";
        static readonly string CallbackUri = "https://stockgazers.kr/api/callback";
#if DEBUG
        static readonly string DanielSrv = "http://127.0.0.1:8000";
#else
        static readonly string DanielSrv = "https://stockgazers.kr";
#endif


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
            return CallbackUri;
        }

        public async Task<HttpResponseMessage> Call(HttpClient client, HttpMethod method, string url, string? param=null)
        {
            string urlparam = param == null || param.Length == 0 ? url : url+"?"+param;
            var request = new HttpRequestMessage(method, urlparam);
            return await client.SendAsync(request);
        }

        public static async Task<bool> RefreshToken(Common common)
        {
            string url = $"https://accounts.stockx.com/oauth/token";
            RefreshToken token = new()
            {
                grant_type = "refresh_token",
                client_id = $"{ClientID}",
                client_secret = $"{ClientSecret}",
                audience = $"gateway.stockx.com",
                refresh_token = $"{common.RefreshToken}"
            };
            var sendData = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            var response = await common.session.PostAsync(url, sendData);
            try
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<JObject>(result);
                common.AccessToken = data["access_token"].ToString();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}

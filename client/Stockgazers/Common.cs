using Stockgazers.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockgazers
{
    public class Common
    {
        public API server = new();
        public HttpClient session = new();

        public bool IsAppLogin = false;

        public string AccessToken = string.Empty;
        public string RefreshToken = string.Empty;
        public string IDToken = string.Empty;
    }
}

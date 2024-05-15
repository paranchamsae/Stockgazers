using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockgazers.Models
{
    public class Token
    {
        public required string grant_type { get; set; }
        public required string client_id { get; set; }
        public required string client_secret { get; set; }
        public required string code { get; set; }
        public required string redirect_uri { get; set; }
    }
}

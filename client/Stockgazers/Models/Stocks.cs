using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockgazers.Models
{
    public class Stock
    {
        public int UserID { get; set; }
        public string IsDelete { get; set; } = string.Empty;
        public string ListingID { get; set; } = string.Empty;
        public string StyleID { get; set; } = string.Empty;
        public string ProductID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string VariantID { get; set; } = string.Empty;
        public string VariantValue { get; set; } = string.Empty;
        public int BuyPrice { get; set; }
        public float BuyPriceUSD { get; set; }
        public int Price { get; set; }
        public int Limit { get; set; }
        public string Status { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public DateTime? SellDatetime { get; set; } = null;
        public DateTime? SendDatetime { get; set; } = null;
        public float AdjustPrice { get; set; }
        public float Profit { get; set; }
    }

    public class Order
    {
        public string OrderNo { get; set; } = string.Empty;
        public string ListingID { get; set; } = string.Empty;
        public int SalePrice { get; set; }
        public double AdjustPrice { get; set; }
    }

    public class CreateListing
    {
        public string amount { get; set; } = string.Empty;
        public string variantId { get; set; } = string.Empty;
    }

    public class RequestPatchListing
    {
        public string ListingID { get; set; } = string.Empty;
        public int BuyPrice { get; set; }
        public int Price { get; set; }
        public int Limit { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class RequestPutListing
    {
        public int amount { get; set; }
    }

    public class AutoPricingData
    {
        public string StyleID { get; set; } = string.Empty;
        public string ListingID { get; set; } = string.Empty;
        public string ProductID { get; set; } = string.Empty;
        public string VariantID { get; set; } = string.Empty;
        public int LimitPrice { get; set; }
        public int BidPrice { get; set; }
    }
}

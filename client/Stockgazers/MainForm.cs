using System.Net;
using Stockgazers.APIs;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Util;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Stockgazers.Models;
using System.Security.Policy;
using System.Net.Http.Headers;
using System.Windows.Forms;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.WebRequestMethods;
using System.Text.RegularExpressions;

#pragma warning disable CS8602
#pragma warning disable CS8604
namespace Stockgazers
{
    public partial class MainForm : MaterialForm
    {
        public static bool isNewStockCreated = false;
        private readonly MaterialSkinManager materialSkinManager;
        Common common;
        System.Windows.Forms.Timer Timer;
        //List<ListViewItem> originCollection;
        List<DataGridViewRow> originCollectionGrid;

        private string authcode = string.Empty;
        private string state = string.Empty;
        public string AuthCode
        {
            get { return authcode; }
            set { authcode = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public MainForm(Common c)
        {
            InitializeComponent();
            common = c;
            //originCollection = new List<ListViewItem>();
            originCollectionGrid = new List<DataGridViewRow>();

            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;

            // Set this to false to disable backcolor enforcing on non-materialSkin components
            // This HAS to be set before the AddFormToManage()
            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            // MaterialSkinManager properties
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            //materialSkinManager.ColorScheme = new MaterialColorScheme(0x00C926b3, 0xA1008B, 0xDC2EFF, 0x006E70FF, MaterialTextShade.LIGHT);
            //materialSkinManager.ColorScheme = new MaterialColorScheme("#00480157", "#370142", "DC2EFF", "00BB5FCF", MaterialTextShade.LIGHT);
            //materialSkinManager.ColorScheme = new MaterialColorScheme(Color.Orange, Color.DarkOrange, Color.Orchid, Color.OrangeRed, Color.MediumOrchid);
            materialSkinManager.ColorScheme = new MaterialColorScheme(MaterialPrimary.Indigo500, MaterialPrimary.Indigo700, MaterialPrimary.Indigo100, MaterialAccent.Pink200, MaterialTextShade.LIGHT);

            Timer = new System.Windows.Forms.Timer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = 30 * 60000;        // 30��
            //Timer.Interval = 1 * 60000;
        }


        private void AppendRunningStatus(string text)
        {
            runningStatusTextEdit.Text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}\r\n" + runningStatusTextEdit.Text;
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            Timer.Stop();
            AppendRunningStatus("���� ��ȸ�� �����մϴ�.");

            #region 1. Stockgazers DB���� ���� ���� ���� ������ �������� ȹ��
            string url = $"{API.GetServer()}/api/stocks/{common.StockgazersUserID}/active";
            var response = await common.session.GetAsync(url);

            List<AutoPricingData> bids = new List<AutoPricingData>();
            try
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                foreach (JToken element in JsonConvert.DeserializeObject<JToken>(result)["data"])
                {
                    bids.Add(new AutoPricingData
                    {
                        StyleID = element["StyleID"].ToString(),
                        ListingID = element["ListingID"].ToString(),
                        ProductID = element["ProductID"].ToString(),
                        VariantID = element["VariantID"].ToString(),
                        LimitPrice = Convert.ToInt32(element["Limit"]),
                        BidPrice = Convert.ToInt32(element["Price"])
                    });
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
                return;
            }
            #endregion

            #region 2. foreach to StockX
            if (bids.Count > 0)
            {
                foreach (AutoPricingData item in bids)
                {
                #region ü��� ������ �ִ��� Ȯ���ϰ� ���°� ������Ʈ ����
                retry_sync:
                    url = $"https://api.stockx.com/v2/selling/listings/{item.ListingID}";
                    response = await common.session.GetAsync(url);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        var tempResult = response.Content.ReadAsStringAsync().Result;
                        var tempResultData = JsonConvert.DeserializeObject<JObject>(tempResult);
                        if (tempResultData["status"].ToString() != "ACTIVE")
                        {
                            Dictionary<string, string> patchData = new()
                            {
                                { "ListingID", item.ListingID },
                                { "Status", tempResultData["status"].ToString() }
                            };
                            url = $"{API.GetServer()}/api/listing/status";
                            var sendData = new StringContent(JsonConvert.SerializeObject(patchData), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);
                            response.EnsureSuccessStatusCode();

                            url = $"https://api.stockx.com/v2/selling/orders/{tempResultData["order"]["orderNumber"]}";
                            response = await common.session.GetAsync(url);
                            response.EnsureSuccessStatusCode();
                            JToken? tempOrder = JsonConvert.DeserializeObject<JToken>(response.Content.ReadAsStringAsync().Result);
                            List<Order> orders = new();
                            Order o = new()
                            {
                                OrderNo = tempOrder["orderNumber"].ToString(),
                                ListingID = tempOrder["listingId"].ToString(),
                                SalePrice = 0,
                                AdjustPrice = 0
                            };
                            orders.Add(o);
                            url = $"{API.GetServer()}/api/stocks/order";
                            sendData = new StringContent(JsonConvert.SerializeObject(orders), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);

                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            if (await API.RefreshToken(common))
                                goto retry_sync;
                            else
                            {
                                MaterialSnackBar snackBar = new("���� ��� ���ſ� �����Ͽ����ϴ�.", "OK", true);
                                snackBar.Show(this);
                                return;
                            }
                        }
                        else
                        {
                            MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                            snackBar.Show(this);
                            return;
                        }
                    }
                    #endregion

                    if (item.LimitPrice == 0)           // ���Ѱ� ������ ���� �ʾҴٸ� ������Ʈ ���� �ʴ´�.
                    {
                        AppendRunningStatus($"{item.StyleID} - ���Ѱ� ������ ����, ���������� ������Ʈ���� �ʽ��ϴ�.");
                        continue;
                    }

                retry:
                    url = $"https://api.stockx.com/v2/catalog/products/{item.ProductID}/variants/{item.VariantID}/market-data";
                    response = await common.session.GetAsync(url);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        var result = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObject<JObject>(result);

                        #region ������
                        /*
                        // ���� StockX�� ���� �������� �� �������� ��
                        int LowestAskAmount = Convert.ToInt32(data["lowestAskAmount"]);
                        int UpdatePrice = -1;
                        // �� ������ ���� ���������(LowestAskAmount == item.BidPrice) ������ ������Ʈ �� �ʿ䰡 ����
                        // �� ������ ���� ���������� �� ���� �����ϼ��� ����(�� ������ LowestAskAmount�� ���ϵ� ���̱� ����)
                        if (LowestAskAmount < item.BidPrice)            // �� �������� ���� �������� �ƴ�
                        {
                            if (LowestAskAmount - 1 > item.LimitPrice)       // $1 ���� ������ ���Ѱ����� ���� ���� ����� ����������-1�ҷ� ���� ������Ʈ
                                UpdatePrice = LowestAskAmount - 1;
                            else if (LowestAskAmount - 1 == item.LimitPrice)
                                UpdatePrice = item.LimitPrice;
                        }
                        //*/
                        #endregion

                        #region �ŷ���
                        /*
                         US/EU ������ ������ ���忡���� lowestAskAmount �ʵ尡 ���ϵ��� �ʴ� ���� �ǵ��� ���̶�� ��.
                        ����ڿ��� �ɼ��� �����ϰ� �ؼ� ���� �������� ���� �۷ι� �������� ���� �����ϰ� �� ������ ������Ʈ�� �����ؾ� �� �� ��
                        (���� �������� �������� �� earnMoreAmount, �۷ι� �������� �����ϸ� sellFasterAmount)
                         */
                        int LowestAskAmount = -1;
                        int UpdatePrice = -1;

                        if (common.DiscountType == "LOCAL")
                            LowestAskAmount = Convert.ToInt32(data["earnMoreAmount"]);
                        else if (common.DiscountType == "GLOBAL")
                            LowestAskAmount = Convert.ToInt32(data["sellFasterAmount"]);

                        if (LowestAskAmount < item.BidPrice)        // �� �������� ���� �������� �ƴ�
                        {
                            if (LowestAskAmount > item.LimitPrice)       // �� ���� ���Ѱ��� ���� ���������ٴ� ���ٸ�
                                UpdatePrice = LowestAskAmount;          // �ش� �������� ������Ʈ
                            else if (LowestAskAmount <= item.LimitPrice)        // ���� ���Ѱ��� ���� ���������� ũ�ų� ���ٸ�
                                UpdatePrice = item.LimitPrice;          // ���� ���Ѱ��� ������Ʈ
                        }
                        #endregion

                        if (UpdatePrice > -1 && UpdatePrice != item.BidPrice)
                        {
                            Thread.Sleep(1500);        // ���� API�� ȣ�� ���̿� 1�� �̻��� ���� �־�� �Ѵ�.
                            // ������ ���� ������Ʈ
                            url = $"https://api.stockx.com/v2/selling/listings/{item.ListingID}";
                            Dictionary<string, string> updateData = new()
                            {
                                { "amount", UpdatePrice.ToString() }
                            };
                            var sendData = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);
                            response.EnsureSuccessStatusCode();

                            // Stockgazers DB���� ������Ʈ
                            url = $"{API.GetServer()}/api/listing/price";
                            updateData.Clear();
                            updateData = new Dictionary<string, string>
                            {
                                { "ListingID", item.ListingID },
                                { "Price", UpdatePrice.ToString() }
                            };
                            sendData = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);
                            response.EnsureSuccessStatusCode();
                            AppendRunningStatus($"{item.StyleID} - ������ ������Ʈ �Ϸ�({item.BidPrice} USD �� {UpdatePrice} USD)");
                        }
                        else
                        {
                            AppendRunningStatus($"{item.StyleID} - ������ �ʿ����� �ʽ��ϴ�.");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            if (await API.RefreshToken(common))
                                goto retry;
                            else
                            {
                                MaterialSnackBar snackBar = new("���� ��� ���ſ� �����Ͽ����ϴ�.", "OK", true);
                                snackBar.Show(this);
                                return;
                            }
                        }
                        else
                        {
                            MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                            snackBar.Show(this);
                            continue;
                        }
                    }
                }
            }
            #endregion

            AppendRunningStatus($"���� ��ȸ �Ϸ�, ���� Ȯ�� �ð��� {DateTime.Now.AddMinutes(Timer.Interval / 60000):HH:mm:ss} �Դϴ�.");
            Timer.Start();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            #region 1-1. ���� �α��� �ϰ� ���� �ڵ带 ȹ����
            string param = $@"response_type=code&client_id={API.ClientID}&redirect_uri={API.GetCallback()}&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            StockXLoginForm stockXLoginForm = new(this, param);
            stockXLoginForm.ShowDialog();

            if (AuthCode == string.Empty || AuthCode.Length <= 0)       // ���� ����(���� �ڵ尡 ����)
            {
                MessageBox.Show("���� �ڵ� ȹ�濡 �����߽��ϴ�.\n���α׷� �� ���� �� StockX �α����� ���� �� �ּ���.", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                materialLabel20.Text = "StockX DB�� ����ȭ�� �� �����ϴ�. ���α׷� �� ������ �ʿ��մϴ�.";
                return;
            }
            #endregion

            #region 1-2. ȹ���� �����ڵ�� ���� �׼���/�������� ��ū�� ����
            if (API.ClientID == null || API.ClientSecret == null)
                return;

            string url = $"https://accounts.stockx.com/oauth/token";
            Token data = new()
            {
                grant_type = "authorization_code",
                client_id = API.ClientID,
                client_secret = API.ClientSecret,
                code = AuthCode,
                redirect_uri = API.GetCallback()
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                "application/json");
            var response = await common.session.PostAsync(url, content);

            try
            {
                response.EnsureSuccessStatusCode();
                string tokenRaw = response.Content.ReadAsStringAsync().Result;
                var tokenJson = JsonConvert.DeserializeObject<JObject>(tokenRaw);
                if (tokenJson != null)
                {
                    JToken Access = tokenJson.Value<string>("access_token");
                    common.AccessToken = Access?.ToString() ?? string.Empty;
                    JToken Refresh = tokenJson.Value<string>("refresh_token");
                    common.RefreshToken = Refresh?.ToString() ?? string.Empty;
                    JToken ID = tokenJson.Value<string>("id_token");
                    common.IDToken = ID?.ToString() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
                return;
            }
            #endregion

            #region 2. �� ��� ����� �������� ������ �ͼ� Ŭ���̾�Ʈ�� �ѷ���
            materialLabel20.Text = "StockX DB�� ����ȭ ���Դϴ�. �Ϻ� �����Ͱ� ��Ȯ�ϰ� ������� ���� �� �ֽ��ϴ�.";
            #region 2-1. ������ ��ϵ� �� ��ü �Ǹ���Ȳ
            url = $"https://api.stockx.com/v2/selling/listings";
            common.session.DefaultRequestHeaders.Add("Authorization", $"Bearer {common.AccessToken}");
            common.session.DefaultRequestHeaders.Add("x-api-key", $"{API.APIKey}");

            response = await common.session.GetAsync(url);
            List<JToken> StockxListingsListOrigin = new List<JToken>();
            try
            {
                response.EnsureSuccessStatusCode();
                var StockxRaw = response.Content.ReadAsStringAsync().Result;
                JToken? tempStockx = JsonConvert.DeserializeObject<JToken>(StockxRaw);
                if (tempStockx != null)
                    StockxListingsListOrigin = JsonConvert.DeserializeObject<JToken>(tempStockx["listings"]!.ToString())!.ToList();

                if (StockxListingsListOrigin.Count == 0)
                {
                    MaterialSnackBar snackBar = new("StockX���� ���� �Ǹ� ��Ȳ�� �ҷ����µ� �����߽��ϴ�.", "OK", true);
                    snackBar.Show(this);
                    return;
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
                return;
            }
            #endregion

            #region 2-2. Stockgazers DB ������ ���� ���� �� ��ü �Ǹ� ��Ȳ
            url = $"{API.GetServer()}/api/stocks/{common.StockgazersUserID}";
            response = await common.session.GetAsync(url);
            JToken? StockgazersReference = null;
            try
            {
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var StockgazersRaw = response.Content.ReadAsStringAsync().Result;
                    StockgazersReference = JsonConvert.DeserializeObject<JToken>(StockgazersRaw);
                }
                if (StockgazersReference == null)
                {
                    MaterialSnackBar snackBar = new("Stockgazers �������� ���� �Ǹ� ��Ȳ�� �ҷ����µ� �����߽��ϴ�.", "OK", true);
                    snackBar.Show(this);
                    return;
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
                return;
            }
            #endregion

            #region 2-3. ���� ����̶� Stockgazers �Ǹ���Ȳ �ߺ� �Ÿ��� �����ִ� �����ʹ� ��� �߰�
            List<JToken> StockxListingsList = StockxListingsListOrigin.Where(x => x["listingId"] != null && x["product"].Any() && x["variant"].Any()).ToList();
            StockxListingsList = StockxListingsList.Where(x => StockgazersReference.Count(y => y["Stocks"]["ListingID"].ToString() == x["listingId"].ToString()) == 0).ToList();
            List<Stock> stocks = new();
            foreach (JToken list in StockxListingsList)
            {
                Stock s = new()
                {
                    UserID = common.StockgazersUserID,
                    IsDelete = "F",
                    ListingID = list["listingId"]!.ToString(),
                    StyleID = list["product"]["styleId"].ToString(),
                    ProductID = list["product"]["productId"].ToString(),
                    Title = list["product"]["productName"].ToString(),
                    VariantID = list["variant"]["variantId"].ToString(),
                    VariantValue = list["variant"]["variantValue"].ToString(),
                    BuyPrice = 0,
                    BuyPriceUSD = 0.0f,
                    Price = Convert.ToInt32(list["amount"].ToString()),
                    Limit = 0,
                    Status = list["status"].ToString(),
                };

                if (list["order"].Count() > 0)
                {
                    s.OrderNo = list["order"]["orderNumber"].ToString();
                    s.SellDatetime = Convert.ToDateTime(list["order"]["orderCreatedAt"]);
                }
                stocks.Add(s);
            }

            if (stocks.Count > 0)
            {
                url = $"{API.GetServer()}/api/stocks";
                var sendData = new StringContent(JsonConvert.SerializeObject(stocks), Encoding.UTF8, "application/json");
                response = await common.session.PostAsync(url, sendData);
            }
        #endregion

        #region 2-4. ������ ACTIVE/MATCHED/READY_TO_SHIP ���¿� ���� �絿��ȭ
        retry:
            List<JToken> tempCompare = StockgazersReference.Where(
                x => x["Stocks"]["Status"].ToString() == "ACTIVE" ||
                x["Stocks"]["Status"].ToString() == "MATCHED" ||
                x["Stocks"]["Status"].ToString() == "READY_TO_SHIP"
            ).ToList();
            bool isActiveRefresh = false;
            foreach (var active in tempCompare)
            {
                url = $"{API.GetServer()}/api/listing/{active["Stocks"]["ListingID"]}";
                response = await common.session.GetAsync(url);
                try
                {
                    response.EnsureSuccessStatusCode();

                    // 240607 price, status ������ Ȯ�����ְ� ������Ʈ ���ָ� ���� �� ����.
                    var result = response.Content.ReadAsStringAsync().Result;
                    var resultData = JsonConvert.DeserializeObject<JToken>(result);
                    int sgPrice = Convert.ToInt32(resultData[0]["Price"].ToString());
                    string sgStatus = resultData[0]["Status"].ToString();

                    url = $"https://api.stockx.com/v2/selling/listings/{active["Stocks"]["ListingID"]}";
                    response = await common.session.GetAsync(url);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        var resultStockx = response.Content.ReadAsStringAsync().Result;
                        var resultDataStockx = JsonConvert.DeserializeObject<JObject>(resultStockx);
                        if (Convert.ToInt32(resultDataStockx["amount"]) != sgPrice || resultDataStockx["status"].ToString() != sgStatus)
                        {
                            RequestPatchListing patchData = new()
                            {
                                ListingID = resultDataStockx["listingId"].ToString(),
                                BuyPrice = Convert.ToInt32(resultData[0]["BuyPrice"]),
                                Price = Convert.ToInt32(resultDataStockx["amount"]),
                                Limit = Convert.ToInt32(resultData[0]["Limit"]),
                                Status = resultDataStockx["status"].ToString()
                            };
                            url = $"{API.GetServer()}/api/listing";
                            var sendData = new StringContent(JsonConvert.SerializeObject(patchData), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);
                            response.EnsureSuccessStatusCode();

                            if (resultDataStockx["order"] != null)
                            {
                                Thread.Sleep(1000);
                                url = $"https://api.stockx.com/v2/selling/orders/{resultDataStockx["order"]["orderNumber"]}";
                                response = await common.session.GetAsync(url);
                                response.EnsureSuccessStatusCode();
                                JToken? tempOrder = JsonConvert.DeserializeObject<JToken>(response.Content.ReadAsStringAsync().Result);
                                List<Order> orders = new();
                                Order o = new()
                                {
                                    OrderNo = tempOrder["orderNumber"].ToString(),
                                    ListingID = tempOrder["listingId"].ToString(),
                                    SalePrice = 0,
                                    AdjustPrice = 0
                                };
                                orders.Add(o);
                                url = $"{API.GetServer()}/api/stocks/order";
                                sendData = new StringContent(JsonConvert.SerializeObject(orders), Encoding.UTF8, "application/json");
                                response = await common.session.PatchAsync(url, sendData);
                            }

                            if (!isActiveRefresh)
                                isActiveRefresh = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            if (await API.RefreshToken(common))
                                goto retry;
                            else
                            {
                                MaterialSnackBar snackBar = new("���� ��� ���ſ� �����Ͽ����ϴ�.", "OK", true);
                                snackBar.Show(this);
                                return;
                            }
                        }
                        else
                        {
                            MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                            snackBar.Show(this);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (await API.RefreshToken(common))
                            goto retry;
                        else
                            return;
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                        snackBar.Show(this);
                        continue;
                    }
                }
            }
            #endregion

            #endregion

            #region 3. Ȩ ȭ�� ������ ������Ʈ - �Ǹ� �Ϸ��̷� ��ȸ(�Ǹűݾ�, ����ݾ� ȹ�� �� �������� profit ���)
            List<JToken> ordersRaw = StockxListingsListOrigin.Where(x => x["order"] != null && x["order"]!.Any()).ToList();         // �������� ������ ��ü ��������
            List<Order> order = new List<Order>();      // ������ �����͸� ������ ����Ʈ
            if (StockgazersReference.Where(x => Convert.ToInt32(x["Stocks"]["AdjustPrice"]) == 0).Any())
            {
                foreach (var row in ordersRaw)
                {
                    var a = StockgazersReference.Where(x => x["Stocks"]["ListingID"].ToString() == row["listingId"].ToString());
                    if (a.Any() && Convert.ToInt32(a.First()["Stocks"]["AdjustPrice"]) == 0 && a.First()["Stocks"]["Status"].ToString() == "COMPLETED")
                    {
                        url = $"https://api.stockx.com/v2/selling/orders/{row["order"]["orderNumber"]}";
                        response = await common.session.GetAsync(url);
                        try
                        {
                            response.EnsureSuccessStatusCode();
                            JToken? tempOrder = JsonConvert.DeserializeObject<JToken>(response.Content.ReadAsStringAsync().Result);
                            Order o = new()
                            {
                                OrderNo = tempOrder["orderNumber"].ToString(),
                                ListingID = tempOrder["listingId"].ToString(),
                                SalePrice = Convert.ToInt32(tempOrder["payout"]["salePrice"]),
                                AdjustPrice = Convert.ToDouble(tempOrder["payout"]["totalPayout"])
                            };
                            order.Add(o);
                        }
                        catch (Exception ex)
                        {
                            MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                            snackBar.Show(this);
                            continue;
                        }
                    }
                }
            }

            if (order.Count > 0)
            {
                url = $"{API.GetServer()}/api/stocks/order";
                var sendData = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                response = await common.session.PatchAsync(url, sendData);
            }

            materialLabel20.Text = "����ȭ �Ϸ�";
            #endregion

            #region 4. �Ǹ���Ȳ �� ����Ʈ�� ������ ����ֱ�
            if (originCollectionGrid?.Count > 0)
                originCollectionGrid.Clear();

            if (order.Count > 0 || isActiveRefresh)
            {
                url = $"{API.GetServer()}/api/stocks/{common.StockgazersUserID}";
                response = await common.session.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var StockgazersRaw = response.Content.ReadAsStringAsync().Result;
                StockgazersReference = JsonConvert.DeserializeObject<JToken>(StockgazersRaw);
            }

            foreach (var row in StockgazersReference)
            {
                string status = string.Empty;
                switch (row["Stocks"]["Status"].ToString())
                {
                    case "ACTIVE":
                        status = "���� ��";
                        break;
                    case "INACTIVE":
                        status = "��Ȱ��ȭ";
                        break;
                    case "DELETED":
                        status = "������";
                        break;
                    case "CANCELED":
                        status = "��������";
                        break;
                    case "MATCHED":
                        status = "�Ǹ�/������";
                        break;
                    case "READY_TO_SHIP":
                        status = "�߼۴��";
                        break;
                    case "COMPLETED":
                        status = "�ǸſϷ�";
                        break;
                }

                //string[] element = new[] {
                //    row["Stocks"]["StyleID"].ToString(),
                //    row["Stocks"]["Title"].ToString(),
                //    $"{row["Variants"]["KRValue"]} ({row["Stocks"]["VariantValue"].ToString()})",
                //    row["Stocks"]["BuyPrice"].ToString(),
                //    row["Stocks"]["Price"].ToString(),
                //    status,
                //    row["Stocks"]["AdjustPrice"].ToString(),
                //    row["Stocks"]["Profit"].ToString(),
                //    row["Stocks"]["CreateDatetime"].ToString(),
                //};
                //ListViewItem item = new ListViewItem(element);
                //item.Tag = row["Stocks"]["ListingID"].ToString();
                //materialListView1.Items.Add(item);

                int newRow = dataGridView1.Rows.Add();
                DataGridViewRow r = dataGridView1.Rows[newRow];
                r.Cells[0].Value = row["Stocks"]["StyleID"].ToString();
                r.Cells[1].Value = row["Stocks"]["Title"].ToString();
                r.Cells[2].Value = $"{row["Variants"]["KRValue"]} ({row["Stocks"]["VariantValue"].ToString()})";
                r.Cells[3].Value = row["Stocks"]["BuyPrice"].ToString();
                r.Cells[4].Value = row["Stocks"]["Price"].ToString();
                r.Cells[5].Value = status;
                r.Cells[6].Value = row["Stocks"]["AdjustPrice"].ToString();
                r.Cells[7].Value = row["Stocks"]["Profit"].ToString();
                r.Cells[8].Value = row["Stocks"]["CreateDatetime"].ToString();
                r.Tag = row["Stocks"]["ListingID"].ToString();

                //originCollection.Add(item);
                originCollectionGrid.Add(r);
            }
            #endregion

            #region 5. ���� �� �Ǹ���� ������ �ҷ��ͼ� ����ֱ�
            GetStatistics(common);
            #endregion

            #region 6. ����� Ƽ� ���� ���ݰ��� Ÿ�̸� ���ۿ��� ����
            if (common.UserTier > 2)
            {
                Timer.Start();
                AppendRunningStatus($"���� ����� Ȯ�� �Ϸ�, ���ݰ��� Ÿ�̸� ���� �Ϸ�\r\n���� Ȯ�� �ð��� {DateTime.Now.AddMinutes(Timer.Interval / 60000):HH:mm:ss} �Դϴ�.");
            }
            #endregion
        }

        private async void GetStatistics(Common common)
        {
            string url = $"{API.GetServer()}/api/main/statistics/{common.StockgazersUserID}";
            var response = await common.session.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
                var resultRaw = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<JObject>(resultRaw);
                data = JsonConvert.DeserializeObject<JObject>(data["data"].ToString());
                materialLabel8.Text = data["TotalRow"].ToString();
                materialLabel9.Text = data["MatchedRow"].ToString();
                materialLabel22.Text = data["ActiveRow"].ToString();
                materialLabel15.Text = $"{data["AvgProfit"].ToString()}%";
                materialLabel21.Text = $"{data["AvgBuyPrice"].ToString()} KRW";
                materialLabel18.Text = $"{data["AvgAdjustPrice"].ToString()} USD";
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
            }
        }

        private void materialRadioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ���ſ��� ���ε�� csv ���� �ٿ�ε�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void materialButton4_Click(object sender, EventArgs e)
        {
            string url = $"{API.GetServer()}/api/data/export/{common.StockgazersUserID}";
            var response = await common.session.GetAsync(url);
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                response.EnsureSuccessStatusCode();
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fs = new(Path.Combine(path, "data.csv"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }

                MaterialSnackBar snackBar = new("���α׷� ��ο� �ٿ�ε� �Ǿ����", "OK", true);
                snackBar.Show(this);
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
            }
        }

        /// <summary>
        /// ���ſ��� csv���� ���ε�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void materialButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                Title = "Stockgazers",
                Filter = "csv ���� (*.csv)|*.csv"
            };

            DialogResult dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                MultipartFormDataContent sendData = new();
                sendData.Headers.ContentType.MediaType = "multipart/form-data";

                string filepath = dlg.FileName;
                try
                {
                    string url = $"{API.GetServer()}/api/data/import";
                    byte[] bytes = System.IO.File.ReadAllBytes(filepath);
                    sendData.Add(new StreamContent(new MemoryStream(bytes)), "csvfile", "data.csv");

                    var response = await common.session.PostAsync(url, sendData);
                    response.EnsureSuccessStatusCode();
                    MaterialSnackBar snackBar2 = new("���ſ��� �����Ͱ� ���ε� �Ǿ����", "OK", true);
                    snackBar2.Show(this);

                    await RefreshSellStatus();          // �Ǹ���Ȳ �׸��� �ٽ� �׸���
                    GetStatistics(common);          // ����ȭ�� ������� ������Ʈ
                }
                catch (IOException)
                {
                    MaterialSnackBar snackBar = new("CSV������ �����־� ���ε� �� �� �����ϴ�.", "OK", true);
                    snackBar.Show(this);
                    return;
                }
                catch (Exception ex)
                {
                    MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                    snackBar.Show(this);
                    return;
                }
            }
            else
                return;
        }

        private async Task<bool> RefreshSellStatus(bool isClearGrid = true)
        {
            if (isClearGrid)
            {
                dataGridView1.Rows.Clear();
                originCollectionGrid.Clear();
            }

            string url = $"{API.GetServer()}/api/stocks/{common.StockgazersUserID}";
            var response = await common.session.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
                JToken? StockgazersReference = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var StockgazersRaw = response.Content.ReadAsStringAsync().Result;
                    StockgazersReference = JsonConvert.DeserializeObject<JToken>(StockgazersRaw);
                }
                if (StockgazersReference == null)
                {
                    MaterialSnackBar snackBar = new("Stockgazers �������� ���� �Ǹ� ��Ȳ�� �ҷ����µ� �����߽��ϴ�.", "OK", true);
                    snackBar.Show(this);
                    return false;
                }

                foreach (var row in StockgazersReference)
                {
                    string status = string.Empty;
                    switch (row["Stocks"]["Status"].ToString())
                    {
                        case "ACTIVE":
                            status = "���� ��";
                            break;
                        case "INACTIVE":
                            status = "��Ȱ��ȭ";
                            break;
                        case "DELETED":
                            status = "������";
                            break;
                        case "CANCELED":
                            status = "��������";
                            break;
                        case "MATCHED":
                            status = "�Ǹ�/������";
                            break;
                        case "READY_TO_SHIP":
                            status = "�߼۴��";
                            break;
                        case "COMPLETED":
                            status = "�ǸſϷ�";
                            break;
                    }

                    int newRow = dataGridView1.Rows.Add();
                    DataGridViewRow r = dataGridView1.Rows[newRow];
                    r.Cells[0].Value = row["Stocks"]["StyleID"].ToString();
                    r.Cells[1].Value = row["Stocks"]["Title"].ToString();
                    r.Cells[2].Value = $"{row["Variants"]["KRValue"]} ({row["Stocks"]["VariantValue"].ToString()})";
                    r.Cells[3].Value = row["Stocks"]["BuyPrice"].ToString();
                    r.Cells[4].Value = row["Stocks"]["Price"].ToString();
                    r.Cells[5].Value = status;
                    r.Cells[6].Value = row["Stocks"]["AdjustPrice"].ToString();
                    r.Cells[7].Value = row["Stocks"]["Profit"].ToString();
                    r.Cells[8].Value = row["Stocks"]["CreateDatetime"].ToString();
                    r.Tag = row["Stocks"]["ListingID"].ToString();

                    originCollectionGrid.Add(r);
                }

                // ���� �� �Ǵ� �Ϸ� ������ư�� ���õ� ����̰ų� �˻�� �ԷµǾ��ִٸ� �׸��带 �ٽ� �׷����Ѵ�.
                if (materialRadioButton7.Checked || materialRadioButton8.Checked || materialTextBoxEdit3.Text.Length > 0)
                {
                    dataGridView1.Rows.Clear();

                    if(materialTextBoxEdit3.Text.Length == 0)
                    {
                        if (materialRadioButton7.Checked)       // ���� �� ��ü
                            dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[5].Value.ToString() == "���� ��" || x.Cells[5].Value.ToString() == "�Ǹ�/������").ToArray());
                        else if (materialRadioButton8.Checked)      // �Ϸ� ��ü
                            dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[5].Value.ToString() == "��������" || x.Cells[5].Value.ToString() == "�ǸſϷ�").ToArray());
                    }
                    else
                    {
                        if (materialRadioButton7.Checked)       // ���� �� ��� �� �ش� �𵨸��� ���� ������
                        {
                            dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => (x.Cells[5].Value.ToString() == "���� ��" || x.Cells[5].Value.ToString() == "�ǸŴ��") &&
                                x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
                        }
                        else if (materialRadioButton8.Checked)      // �Ϸ� ��� �� �ش� �𵨸��� ���� ������
                        {
                            dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => (x.Cells[5].Value.ToString() == "��������" || x.Cells[5].Value.ToString() == "�ǸſϷ�") &&
                                x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
                        }
                        else
                            dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[0].ErrorText.ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
                    }
                }

                dataGridView1.Invalidate();

                return true;
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new($"RefreshSellStatus: {ex.Message}", "OK", true);
                snackBar.Show(this);
                return false;
            }
        }

        private async void materialFloatingActionButton1_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(common);
            addStockForm.ShowDialog();

            if (isNewStockCreated)
            {
                isNewStockCreated = false;
                MaterialSnackBar snackBar = new($"��� �߰��� �Ϸ�Ǿ����", "OK", true);
                snackBar.Show(this);

                await RefreshSellStatus();
            }
        }


        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ModifyStockForm modifyStockForm = new ModifyStockForm(common, contextMenuStrip1.Items[0].Tag.ToString());
            modifyStockForm.ShowDialog();

            if (isNewStockCreated)
            {
                isNewStockCreated = false;
                MaterialSnackBar snackBar = new($"���� ������ �Ϸ�Ǿ����", "OK", true);
                snackBar.Show(this);

                await RefreshSellStatus();
            }
        }

        private async void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string message = $"{contextMenuStrip1.Items[0].Text} ���� ���� ����� �����ұ��?";
            if (MessageBox.Show(message, "���� ��� ����", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
            retry:
                #region ���� ������� ����
                string ListingID = contextMenuStrip1.Items[0].Tag.ToString() ?? string.Empty;
                if (ListingID.Length == 0)
                    return;

                string url = $"https://api.stockx.com/v2/selling/listings/{ListingID}";
                var response = await common.session.DeleteAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (await API.RefreshToken(common))
                            goto retry;
                        else
                        {
                            MaterialSnackBar snackBar = new("���� ��� ���ſ� �����Ͽ����ϴ�.", "OK", true);
                            snackBar.Show(this);
                            return;
                        }
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }
                #endregion

                #region Stockgazers DB ������Ʈ
                url = $"{API.GetServer()}/api/listing/{ListingID}";
                response = await common.session.DeleteAsync(url);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    MaterialSnackBar snackBar = new($"�������� ��ſ� �����߾�� :(", "OK", true);
                    snackBar.Show(this);
                    return;
                }
                #endregion

                await RefreshSellStatus();
            }
        }

        private async void materialButton6_Click(object sender, EventArgs e)
        {
            await RefreshSellStatus();
        }



        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            //materialListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                DataGridViewRow item = dataGridView1.Rows[rowIndex];
                if (item != null)
                {
                    item.Selected = true;

                    contextMenuStrip1.Items[0].Text = item.Cells[1].Value.ToString();
                    contextMenuStrip1.Items[0].Tag = item.Tag;
                    contextMenuStrip1.Items[0].Enabled = false;

                    if (item.Cells[5].Value.ToString() == "���� ��")
                    {
                        contextMenuStrip1.Items[1].Enabled = true;
                        contextMenuStrip1.Items[2].Enabled = true;
                    }
                    else
                    {
                        contextMenuStrip1.Items[1].Enabled = false;
                        contextMenuStrip1.Items[2].Enabled = false;
                    }

                    contextMenuStrip1.Show(dataGridView1, e.Location);
                }
            }
        }

        /// <summary>
        /// ���� ������Ȳ ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialButton8_Click(object sender, EventArgs e)
        {
            panel4.Visible = true;
        }

        /// <summary>
        /// ���ſ��� ������ �ٿ�ε�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void materialButton10_Click(object sender, EventArgs e)
        {
            string url = $"{API.GetServer()}/api/data/export/{common.StockgazersUserID}";
            var response = await common.session.GetAsync(url);
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            try
            {
                response.EnsureSuccessStatusCode();
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fs = new(Path.Combine(path, "data.csv"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }

                MaterialSnackBar snackBar = new("���α׷� ��ο� �ٿ�ε� �Ǿ����", "OK", true);
                snackBar.Show(this);
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
            }
        }

        /// <summary>
        /// ���ſ��� ������ ���ε�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void materialButton9_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                Title = "Stockgazers",
                Filter = "csv ���� (*.csv)|*.csv"
            };

            DialogResult dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                MultipartFormDataContent sendData = new();
                sendData.Headers.ContentType.MediaType = "multipart/form-data";

                string filepath = dlg.FileName;
                try
                {
                    string url = $"{API.GetServer()}/api/data/import";
                    byte[] bytes = System.IO.File.ReadAllBytes(filepath);
                    sendData.Add(new StreamContent(new MemoryStream(bytes)), "csvfile", "data.csv");

                    var response = await common.session.PostAsync(url, sendData);
                    response.EnsureSuccessStatusCode();
                    MaterialSnackBar snackBar2 = new("���ſ��� �����Ͱ� ���ε� �Ǿ����", "OK", true);
                    snackBar2.Show(this);

                    await RefreshSellStatus();          // �Ǹ���Ȳ �׸��� �ٽ� �׸���
                    GetStatistics(common);          // ����ȭ�� ������� ������Ʈ
                }
                catch (IOException)
                {
                    MaterialSnackBar snackBar = new("CSV������ �����־� ���ε� �� �� �����ϴ�.", "OK", true);
                    snackBar.Show(this);
                    return;
                }
                catch (Exception ex)
                {
                    MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                    snackBar.Show(this);
                    return;
                }
            }
            else
                return;
        }

        /// <summary>
        /// ������ư - ��ü
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialRadioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton6.Checked)
            {
                dataGridView1.Rows.Clear();

                if (materialTextBoxEdit3.Text.Length > 0)
                {
                    dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
                }
                else
                {
                    foreach (var element in originCollectionGrid)
                        dataGridView1.Rows.Add(element);
                }
            }
        }

        /// <summary>
        /// ������ư - ���� ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialRadioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton7.Checked)
            {
                dataGridView1.Rows.Clear();

                IEnumerable<DataGridViewRow> filteredItem = originCollectionGrid.Where(x => x.Cells[5].Value.ToString() == "���� ��" || x.Cells[5].Value.ToString() == "�Ǹ�/������");
                if (materialTextBoxEdit3.Text.Length > 0)
                    filteredItem = filteredItem.Where(x => x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text));

                dataGridView1.Rows.AddRange(filteredItem.ToArray());
            }
        }

        /// <summary>
        /// ������ư - �Ϸ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialRadioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton8.Checked)
            {
                dataGridView1.Rows.Clear();

                IEnumerable<DataGridViewRow> filteredItem = originCollectionGrid.Where(x => x.Cells[5].Value.ToString() == "��������" || x.Cells[5].Value.ToString() == "�ǸſϷ�");
                if (materialTextBoxEdit3.Text.Length > 0)
                    filteredItem = filteredItem.Where(x => x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text));

                dataGridView1.Rows.AddRange(filteredItem.ToArray());
            }
        }

        /// <summary>
        /// ���� ������Ȳ ���� �ݱ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialButton6_Click_1(object sender, EventArgs e)
        {
            panel4.Visible = false;
        }

        private void materialTextBoxEdit3_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (materialTextBoxEdit3.Text == string.Empty)
            {
                // ������ư�� ����� ������ ���
                if (materialRadioButton7.Checked)
                {
                    dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[5].Value.ToString() == "���� ��" || x.Cells[5].Value.ToString() == "�Ǹ�/������").ToArray());
                }
                else if (materialRadioButton8.Checked)
                {
                    dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[5].Value.ToString() == "��������" || x.Cells[5].Value.ToString() == "�ǸſϷ�").ToArray());
                }
                else
                {
                    foreach (var element in originCollectionGrid)
                        dataGridView1.Rows.Add(element);
                }
            }
            else
            {
                // ������ư�� ����� ������ ��� = ������ư + �˻��� ������ø
                if (materialRadioButton7.Checked)
                    dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => (x.Cells[5].Value.ToString() == "���� ��" || x.Cells[5].Value.ToString() == "�ǸŴ��") &&
                        x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
                else if (materialRadioButton8.Checked)
                    dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => (x.Cells[5].Value.ToString() == "��������" || x.Cells[5].Value.ToString() == "�ǸſϷ�") &&
                        x.Cells[0].Value.ToString().ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
                else
                    dataGridView1.Rows.AddRange(originCollectionGrid.Where(x => x.Cells[0].ErrorText.ToLower().Replace("-", "").Contains(materialTextBoxEdit3.Text)).ToArray());
            }
        }


        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            //int rowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            //int columnIndex = dataGridView1.HitTest(e.X, e.Y).ColumnIndex;
            //if (rowIndex == -1 || columnIndex != 3) return;

            //DataGridViewRow selected = dataGridView1.Rows[rowIndex];
            //MessageBox.Show(selected.Cells[columnIndex].Value.ToString());
        }

        DataGridViewCell? selectedEdit = null;
        string originSelectedValue = string.Empty;
        bool isBeginEdit = false;
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex != 3)
                return;

            selectedEdit = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            dataGridView1.CurrentCell = selectedEdit;
            originSelectedValue = selectedEdit.Value.ToString();
            dataGridView1.BeginEdit(true);
            isBeginEdit = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // ���ſ��� �����͸� �����ߴ��� ��������� ����Ű�� ������ �ʴ´ٸ� ���� ������� �ʴ´�.
            if (isBeginEdit)
            {
                dataGridView1.EndEdit();
                isBeginEdit = false;

                DataGridViewRow recoveryRow = originCollectionGrid.Where(x => x.Index == selectedEdit.RowIndex).First();
                dataGridView1.Rows[selectedEdit.RowIndex].Cells[selectedEdit.ColumnIndex].Value = originSelectedValue;
                selectedEdit = null;
                originSelectedValue = string.Empty;
            }
        }

        private async void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (isBeginEdit)
            {
                dataGridView1.EndEdit();
                isBeginEdit = false;

                if (e.KeyCode == Keys.Escape)        // ESC Ű�� �Է� ���� ���� ���� ������ ����ϰ� ���� ����
                {
                    DataGridViewRow recoveryRow = originCollectionGrid.Where(x => x.Index == selectedEdit.RowIndex).First();
                    dataGridView1.Rows[selectedEdit.RowIndex].Cells[selectedEdit.ColumnIndex].Value = originSelectedValue;
                }
                else if (e.KeyCode == Keys.Enter)       // ���� ���� �ݿ�
                {
                    DataGridViewRow targetRow = originCollectionGrid.Where(x => x.Index == selectedEdit.RowIndex).First();
                    
                    // ���ڸ� �ԷµǾ��°�?
                    Regex regex = new Regex("^[0-9]*$");
                    if (!regex.IsMatch(targetRow.Cells[selectedEdit.ColumnIndex].Value.ToString()))
                    {
                        dataGridView1.Rows[selectedEdit.RowIndex].Cells[selectedEdit.ColumnIndex].Value = originSelectedValue;
                        MaterialSnackBar snackBar = new("�ùٸ��� ���� ���� �ԷµǾ����", "OK", true);
                        snackBar.Show(this);
                    }
                    else
                    {
                        string url = $"{API.GetServer()}/api/stocks/buyprice";
                        Dictionary<string, string> data = new Dictionary<string, string>()
                        {
                            {"ListingID", targetRow.Tag.ToString() },
                            {"BuyPrice", targetRow.Cells[selectedEdit.ColumnIndex].Value.ToString() }
                        };
                        var sendData = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        var response = await common.session.PatchAsync(url, sendData);
                        try
                        {
                            response.EnsureSuccessStatusCode();
                            MaterialSnackBar snackBar = new("���ſ��� ������ ������Ʈ�� �Ϸ�Ǿ����", "OK", true);
                            snackBar.Show(this);
                        }
                        catch (Exception ex)
                        {
                            MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                            snackBar.Show(this);
                        }
                    }
                }
                selectedEdit = null;
                originSelectedValue = string.Empty;
            }
        }
    }
}
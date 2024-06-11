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
        List<ListViewItem> originCollection;

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
            originCollection = new List<ListViewItem>();

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
            Timer.Interval = 30 * 60000;        // 30분
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            #region 1. Stockgazers DB에서 현재 입찰 중인 상태의 아이템을 획득
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
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {

                }
                else
                {
                    MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                    snackBar.Show(this);
                    return;
                }
            }
            #endregion

            #region 2. foreach to StockX
            if (bids.Count > 0)
            {
                foreach (AutoPricingData item in bids)
                {
                    if (item.LimitPrice == 0)           // 하한가 설정이 되지 않았다면 업데이트 하지 않는다.
                        continue;

                    url = $"https://api.stockx.com/v2/catalog/products/{item.ProductID}/variants/{item.VariantID}/market-data";
                    response = await common.session.GetAsync(url);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        var result = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObject<JObject>(result);

                        #region 구로직
                        /*
                        // 현재 StockX의 최저 입찰가와 내 입찰가를 비교
                        int LowestAskAmount = Convert.ToInt32(data["lowestAskAmount"]);
                        int UpdatePrice = -1;
                        // 내 가격이 현재 최저가라면(LowestAskAmount == item.BidPrice) 가격을 업데이트 할 필요가 없음
                        // 내 가격이 현재 최저가보다 더 낮은 가격일수는 없음(그 가격이 LowestAskAmount로 리턴될 것이기 때문)
                        if (LowestAskAmount < item.BidPrice)            // 내 입찰가가 현재 최저가가 아님
                        {
                            if (LowestAskAmount - 1 > item.LimitPrice)       // $1 낮춘 입찰이 하한가보다 낮지 않은 경우라면 최저입찰가-1불로 입찰 업데이트
                                UpdatePrice = LowestAskAmount - 1;
                            else if (LowestAskAmount - 1 == item.LimitPrice)
                                UpdatePrice = item.LimitPrice;
                        }
                        //*/
                        #endregion

                        #region 신로직
                        /*
                         US/EU 마켓을 제외한 시장에서는 lowestAskAmount 필드가 리턴되지 않는 것이 의도된 것이라고 함.
                        사용자에게 옵션을 선택하게 해서 지역 최저가로 갈지 글로벌 최저가로 갈지 결정하게 한 다음에 업데이트를 진행해야 할 듯 함
                        (지역 최저가로 선택했을 때 earnMoreAmount, 글로벌 최저가로 선택하면 sellFasterAmount)
                         */
                        int LowestAskAmount = -1;
                        int UpdatePrice = -1;

                        if (common.DiscountType == "LOCAL")
                            LowestAskAmount = Convert.ToInt32(data["earnMoreAmount"]);
                        else if (common.DiscountType == "GLOBAL")
                            LowestAskAmount = Convert.ToInt32(data["sellFasterAmount"]);

                        if (LowestAskAmount < item.BidPrice)        // 내 입찰가가 현재 최저가가 아님
                        {
                            if (LowestAskAmount > item.LimitPrice)       // 내 입찰 하한가가 현재 최저가보다는 높다면
                                UpdatePrice = LowestAskAmount;          // 해당 최저가로 업데이트
                            else if (LowestAskAmount <= item.LimitPrice)        // 입찰 하한가가 현재 최저가보다 크거나 같다면
                                UpdatePrice = item.LimitPrice;          // 입찰 하한가로 업데이트
                        }
                        #endregion

                        if (UpdatePrice > -1)
                        {
                            // 딱스에 입찰 업데이트
                            url = $"https://api.stockx.com/v2/selling/listings/{item.ListingID}";
                            Dictionary<string, string> updateData = new()
                            {
                                { "amount", UpdatePrice.ToString() }
                            };
                            var sendData = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);
                            response.EnsureSuccessStatusCode();

                            // Stockgazers DB에도 업데이트
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
                        }
                    }
                    catch (Exception ex)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {

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
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            #region 1-1. 딱스 로그인 하고 인증 코드를 획득함
            string param = $@"response_type=code&client_id={API.ClientID}&redirect_uri={API.GetCallback()}&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            StockXLoginForm stockXLoginForm = new(this, param);
            stockXLoginForm.ShowDialog();

            if (AuthCode == string.Empty || AuthCode.Length <= 0)       // 인증 실패(인증 코드가 없음)
            {
                MessageBox.Show("인증 코드 획득 실패, 딱스 재로그인 필요", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region 1-2. 획득한 인증코드로 딱스 액세스/리프레시 토큰을 얻음
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
            #endregion

            #region 2. 내 재고 목록을 서버에서 가지고 와서 클라이언트에 뿌려줌
            materialLabel20.Text = "StockX DB와 동기화 중입니다. 일부 데이터가 정확하게 노출되지 않을 수 있습니다.";
            #region 2-1. 딱스에 등록된 내 전체 판매현황
            url = $"https://api.stockx.com/v2/selling/listings";
            common.session.DefaultRequestHeaders.Add("Authorization", $"Bearer {common.AccessToken}");
            common.session.DefaultRequestHeaders.Add("x-api-key", $"{API.APIKey}");

            response = await common.session.GetAsync(url);
            List<JToken> StockxListingsListOrigin = new List<JToken>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var StockxRaw = response.Content.ReadAsStringAsync().Result;
                JToken? tempStockx = JsonConvert.DeserializeObject<JToken>(StockxRaw);
                if (tempStockx != null)
                    StockxListingsListOrigin = JsonConvert.DeserializeObject<JToken>(tempStockx["listings"]!.ToString())!.ToList();
            }
            if (StockxListingsListOrigin.Count == 0)
            {
                MaterialSnackBar snackBar = new("StockX에서 나의 판매 현황을 불러오는데 실패했습니다.", "OK", true);
                snackBar.Show(this);
                return;
            }
            #endregion

            #region 2-2. Stockgazers DB 서버에 관리 중인 내 전체 판매 현황
            url = $"{API.GetServer()}/api/stocks/{common.StockgazersUserID}";
            response = await common.session.GetAsync(url);
            JToken? StockgazersReference = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var StockgazersRaw = response.Content.ReadAsStringAsync().Result;
                StockgazersReference = JsonConvert.DeserializeObject<JToken>(StockgazersRaw);
            }
            if (StockgazersReference == null)
            {
                MaterialSnackBar snackBar = new("Stockgazers 서버에서 나의 판매 현황을 불러오는데 실패했습니다.", "OK", true);
                snackBar.Show(this);
            }
            #endregion

            #region 2-3. 딱스 목록이랑 Stockgazers 판매현황 중복 거르고 남아있는 데이터는 디비에 추가
            List<JToken> StockxListingsList = StockxListingsListOrigin.Where(x => x["listingId"] != null && x["product"].Any() && x["variant"].Any()).ToList();
            StockxListingsList = StockxListingsList.Where(x => StockgazersReference.Count(y => y["ListingID"].ToString() == x["listingId"].ToString()) == 0).ToList();
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

            #region 2-4. 디비상의 ACTIVE 상태에 대해 재동기화
            retry:
            List<JToken> tempCompare = StockxListingsListOrigin.Where(x => x["status"].ToString() == "ACTIVE").ToList();
            foreach (var active in tempCompare)
            {
                url = $"{API.GetServer()}/api/listing/{active["listingId"]}";
                response = await common.session.GetAsync(url);
                try
                {
                    response.EnsureSuccessStatusCode();

                    // 240607 price, status 정도만 확인해주고 업데이트 해주면 좋을 것 같다.
                    var result = response.Content.ReadAsStringAsync().Result;
                    var resultData = JsonConvert.DeserializeObject<JToken>(result);
                    int sgPrice = Convert.ToInt32(resultData[0]["Price"].ToString());
                    string sgStatus = resultData[0]["Status"].ToString();

                    url = $"https://api.stockx.com/v2/selling/listings/{active["listingId"]}";
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
                                Limit = Convert.ToInt32(resultData[0]["Limit"])
                            };
                            url = $"{API.GetServer()}/api/listing";
                            var sendData = new StringContent(JsonConvert.SerializeObject(patchData), Encoding.UTF8, "application/json");
                            response = await common.session.PatchAsync(url, sendData);
                            response.EnsureSuccessStatusCode();
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
                catch(Exception ex)
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

            #region 3. 홈 화면 데이터 업데이트 - 판매 완료이력 조회(판매금액, 정산금액 획득 후 서버에서 profit 계산)
            List<JToken> ordersRaw = StockxListingsListOrigin.Where(x => x["order"] != null && x["order"]!.Any()).ToList();         // 딱스에서 가져온 전체 입찰정보
            List<Order> order = new List<Order>();      // 갱신할 데이터를 저장할 리스트
            if (StockgazersReference.Where(x => Convert.ToInt32(x["AdjustPrice"]) == 0).Any())
            {
                foreach (var row in ordersRaw)
                {
                    var a = StockgazersReference.Where(x => x["ListingID"].ToString() == row["listingId"].ToString());
                    if (a.Any() && Convert.ToInt32(a.First()["AdjustPrice"]) == 0 && a.First()["Status"].ToString() == "COMPLETED")
                    {
                        url = $"https://api.stockx.com/v2/selling/orders/{row["order"]["orderNumber"]}";
                        response = await common.session.GetAsync(url);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
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
                    }
                }
            }

            if (order.Count > 0)
            {
                url = $"{API.GetServer()}/api/stocks/order";
                var sendData = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                response = await common.session.PatchAsync(url, sendData);
            }

            materialLabel20.Text = "동기화 완료";
            #endregion

            #region 4. 판매현황 탭 리스트뷰 데이터 집어넣기
            if (originCollection?.Count > 0)
                originCollection.Clear();

            foreach (var row in StockgazersReference)
            {
                string status = string.Empty;
                switch (row["Status"].ToString())
                {
                    case "ACTIVE":
                        status = "입찰 중";
                        break;
                    case "INACTIVE":
                        status = "비활성화";
                        break;
                    case "DELETED":
                        status = "삭제됨";
                        break;
                    case "CANCELED":
                        status = "인증실패";
                        break;
                    case "MATCHED":
                        status = "판매대기";
                        break;
                    case "COMPLETED":
                        status = "판매완료";
                        break;
                }

                string[] element = new[] {
                    row["StyleId"].ToString(),
                    row["Title"].ToString(),
                    row["BuyPrice"].ToString(),
                    row["Price"].ToString(),
                    row["CreateDatetime"].ToString(),
                    status,
                    row["AdjustPrice"].ToString(),
                    row["Profit"].ToString(),
                };
                ListViewItem item = new ListViewItem(element);
                item.Tag = row["ListingID"].ToString();
                materialListView1.Items.Add(item);

                originCollection.Add(item);
            }
            #endregion

            #region 5. 메인 탭 판매통계 데이터 불러와서 집어넣기
            GetStatistics(common);
            #endregion

            #region 6. 사용자 티어에 따라 가격경쟁 타이머 시작여부 결정
            //if (common.UserTier > 2)
            //    //Timer.Start();
            //    //TimerFuncTest();
            //    await API.RefreshToken(common);
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
        /// 구매원가 업로드용 csv 파일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void materialButton4_Click(object sender, EventArgs e)
        {
            string url = $"{API.GetServer()}/api/data/export/{common.StockgazersUserID}";
            var response = await common.session.GetAsync(url);
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    using (FileStream fs = new(Path.Combine(path, "data.csv"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }

                MaterialSnackBar snackBar = new("프로그램 경로에 다운로드 되었어요", "OK", true);
                snackBar.Show(this);
            }
        }

        /// <summary>
        /// 구매원가 csv파일 업로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void materialButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                Title = "Stockgazers",
                Filter = "csv 파일 (*.csv)|*.csv"
            };

            DialogResult dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                MultipartFormDataContent sendData = new();
                sendData.Headers.ContentType.MediaType = "multipart/form-data";

                string filepath = dlg.FileName;
                byte[] bytes = System.IO.File.ReadAllBytes(filepath);
                sendData.Add(new StreamContent(new MemoryStream(bytes)), "csvfile", "data.csv");

                var response = await common.session.PostAsync(API.GetServer() + "/api/stocks/import", sendData);
                try
                {
                    response.EnsureSuccessStatusCode();
                    MaterialSnackBar snackBar2 = new("구매원가 데이터가 업로드 되었어요", "OK", true);
                    snackBar2.Show(this);

                    await RefreshSellStatus();
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
                materialListView1.Items?.Clear();
                originCollection?.Clear();
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
                    MaterialSnackBar snackBar = new("Stockgazers 서버에서 나의 판매 현황을 불러오는데 실패했습니다.", "OK", true);
                    snackBar.Show(this);
                }

                foreach (var row in StockgazersReference)
                {
                    string status = string.Empty;
                    switch (row["Status"].ToString())
                    {
                        case "ACTIVE":
                            status = "입찰 중";
                            break;
                        case "INACTIVE":
                            status = "비활성화";
                            break;
                        case "DELETED":
                            status = "삭제됨";
                            break;
                        case "CANCELED":
                            status = "인증실패";
                            break;
                        case "MATCHED":
                            status = "판매대기";
                            break;
                        case "COMPLETED":
                            status = "판매완료";
                            break;
                    }

                    string[] element = new[] {
                        //"",
                        row["StyleId"].ToString(),
                        row["Title"].ToString(),
                        row["BuyPrice"].ToString(),
                        row["Price"].ToString(),
                        row["CreateDatetime"].ToString(),
                        status,
                        row["AdjustPrice"].ToString(),
                        row["Profit"].ToString(),
                    };
                    ListViewItem item = new ListViewItem(element);
                    item.Tag = row["ListingID"].ToString();
                    materialListView1.Items.Add(item);

                    originCollection ??= new List<ListViewItem>();
                    originCollection.Add(item);
                }

                materialListView1.Invalidate();
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
                MaterialSnackBar snackBar = new($"재고 추가가 완료되었어요", "OK", true);
                snackBar.Show(this);

                await RefreshSellStatus();
            }
        }

        private void materialListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem item = materialListView1.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    item.Selected = true;

                    contextMenuStrip1.Items[0].Text = item.SubItems[1].Text;
                    contextMenuStrip1.Items[0].Tag = item.Tag;
                    contextMenuStrip1.Items[0].Enabled = false;

                    if (item.SubItems[5].Text == "판매완료")
                    {
                        contextMenuStrip1.Items[1].Enabled = false;
                        contextMenuStrip1.Items[2].Enabled = false;
                    }
                    else
                    {
                        contextMenuStrip1.Items[1].Enabled = true;
                        contextMenuStrip1.Items[2].Enabled = true;
                    }

                    contextMenuStrip1.Show(materialListView1, e.Location);
                }
            }
        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ModifyStockForm modifyStockForm = new ModifyStockForm(common, contextMenuStrip1.Items[0].Tag.ToString());
            modifyStockForm.ShowDialog();

            if (isNewStockCreated)
            {
                isNewStockCreated = false;
                MaterialSnackBar snackBar = new($"입찰 수정이 완료되었어요", "OK", true);
                snackBar.Show(this);

                await RefreshSellStatus();
            }
        }

        private async void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string message = $"{contextMenuStrip1.Items[0].Text} 모델의 입찰 등록을 삭제할까요?";
            if (MessageBox.Show(message, "입찰 등록 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                #region 딱스 입찰등록 삭제
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

                    }
                    else
                    {
                        MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }
                #endregion

                #region Stockgazers DB 업데이트
                url = $"{API.GetServer()}/api/listing/{ListingID}";
                response = await common.session.DeleteAsync(url);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    MaterialSnackBar snackBar = new($"서버와의 통신에 실패했어요 :(", "OK", true);
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

        private void materialTextBoxEdit2_TextChanged(object sender, EventArgs e)
        {
            materialListView1.Items.Clear();
            if (materialTextBoxEdit2.Text == string.Empty)
            {
                // 라디오버튼이 적용된 상태의 경우
                if (materialRadioButton4.Checked)
                {
                    materialListView1.Items.AddRange(originCollection.Where(x => x.SubItems[5].Text == "입찰 중" || x.SubItems[5].Text == "판매대기").ToArray());
                }
                else if (materialRadioButton5.Checked)
                {
                    materialListView1.Items.AddRange(originCollection.Where(x => x.SubItems[5].Text == "인증실패" || x.SubItems[5].Text == "판매완료").ToArray());
                }
                else
                {
                    foreach (var element in originCollection)
                        materialListView1.Items.Add(element);
                }
            }
            else
            {
                // 라디오버튼이 적용된 상태의 경우 = 라디오버튼 + 검색어 이중중첩
                if (materialRadioButton4.Checked)
                    materialListView1.Items.AddRange(originCollection.Where(x => (x.SubItems[5].Text == "입찰 중" || x.SubItems[5].Text == "판매대기") &&
                        x.SubItems[1].Text.ToLower().Replace("-", "").Contains(materialTextBoxEdit2.Text)).ToArray());
                else if (materialRadioButton5.Checked)
                    materialListView1.Items.AddRange(originCollection.Where(x => (x.SubItems[5].Text == "인증실패" || x.SubItems[5].Text == "판매완료") &&
                        x.SubItems[1].Text.ToLower().Replace("-", "").Contains(materialTextBoxEdit2.Text)).ToArray());
                else
                    materialListView1.Items.AddRange(originCollection.Where(x => x.SubItems[1].Text.ToLower().Replace("-", "").Contains(materialTextBoxEdit2.Text)).ToArray());
            }
        }

        /// <summary>
        /// 라디오버튼 - 전체
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton3.Checked)
            {
                materialListView1.Items.Clear();

                
                if (materialTextBoxEdit2.Text.Length > 0)
                {
                    materialListView1.Items.AddRange(originCollection.Where(x => x.SubItems[1].Text.ToLower().Replace("-", "").Contains(materialTextBoxEdit2.Text)).ToArray());
                }
                else
                {
                    foreach (var element in originCollection)
                        materialListView1.Items.Add(element);
                }
            }
        }

        /// <summary>
        /// 라디오버튼 - 입찰 중
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialRadioButton4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (materialRadioButton4.Checked)
            {
                materialListView1.Items.Clear();

                IEnumerable<ListViewItem> filteredItem = originCollection.Where(x => x.SubItems[5].Text == "입찰 중" || x.SubItems[5].Text == "판매대기");
                if (materialTextBoxEdit2.Text.Length > 0)
                    filteredItem = filteredItem.Where(x => x.SubItems[1].Text.ToLower().Replace("-", "").Contains(materialTextBoxEdit2.Text));

                materialListView1.Items.AddRange(filteredItem.ToArray());
            }
        }

        /// <summary>
        /// 라디오버튼 - 완료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialRadioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton5.Checked)
            {
                materialListView1.Items.Clear();

                IEnumerable<ListViewItem> filteredItem = originCollection.Where(x => x.SubItems[5].Text == "인증실패" || x.SubItems[5].Text == "판매완료");
                if (materialTextBoxEdit2.Text.Length > 0)
                    filteredItem = filteredItem.Where(x => x.SubItems[1].Text.ToLower().Replace("-", "").Contains(materialTextBoxEdit2.Text));

                materialListView1.Items.AddRange(filteredItem.ToArray());
            }
        }
    }
}
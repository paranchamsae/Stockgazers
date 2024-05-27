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

namespace Stockgazers
{
#pragma warning disable CS8602
#pragma warning disable CS8604
    public partial class MainForm : MaterialForm
    {
        public static bool isNewStockCreated = false;
        private readonly MaterialSkinManager materialSkinManager;
        Common common;

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

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            #region 1-1. ���� �α��� �ϰ� ���� �ڵ带 ȹ����
            string param = $@"response_type=code&client_id={API.ClientID}&redirect_uri={API.GetCallback()}&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            StockXLoginForm stockXLoginForm = new(this, param);
            stockXLoginForm.ShowDialog();

            if (AuthCode == string.Empty || AuthCode.Length <= 0)       // ���� ����(���� �ڵ尡 ����)
            {
                MessageBox.Show("���� �ڵ� ȹ�� ����, ���� ��α��� �ʿ�", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                Trace.WriteLine(API.APIKey);
                Trace.WriteLine(common.AccessToken);
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
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var StockxRaw = response.Content.ReadAsStringAsync().Result;
                JToken? tempStockx = JsonConvert.DeserializeObject<JToken>(StockxRaw);
                if (tempStockx != null)
                    StockxListingsListOrigin = JsonConvert.DeserializeObject<JToken>(tempStockx["listings"]!.ToString())!.ToList();
            }
            if (StockxListingsListOrigin.Count == 0)
            {
                MaterialSnackBar snackBar = new("StockX���� ���� �Ǹ� ��Ȳ�� �ҷ����µ� �����߽��ϴ�.", "OK", true);
                snackBar.Show(this);
                return;
            }
            #endregion

            #region 2-2. Stockgazers DB ������ ���� ���� �� ��ü �Ǹ� ��Ȳ
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
                MaterialSnackBar snackBar = new("Stockgazers �������� ���� �Ǹ� ��Ȳ�� �ҷ����µ� �����߽��ϴ�.", "OK", true);
                snackBar.Show(this);
            }
            #endregion

            #region 2-3. ���� ����̶� Stockgazers �Ǹ���Ȳ �ߺ� �Ÿ��� �����ִ� �����ʹ� ��� �߰�
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

            #endregion

            #region 3. Ȩ ȭ�� ������ ������Ʈ - �Ǹ� �Ϸ��̷� ��ȸ(�Ǹűݾ�, ����ݾ� ȹ�� �� �������� profit ���)
            List<JToken> ordersRaw = StockxListingsListOrigin.Where(x => x["order"] != null && x["order"]!.Any()).ToList();
            List<Order> order = new List<Order>();
            if (StockgazersReference.Where(x => Convert.ToInt32(x["Price"]) == 0).Any())
            {
                foreach (var row in ordersRaw)
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

            if (order.Count > 0)
            {
                url = $"{API.GetServer()}/api/stocks/order";
                var sendData = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                response = await common.session.PatchAsync(url, sendData);
            }

            materialLabel20.Text = "����ȭ �Ϸ�";
            #endregion

            #region 4. �Ǹ���Ȳ �� ����Ʈ�� ������ ����ֱ�
            foreach (var row in StockgazersReference)
            {
                string[] element = new[] {
                    "",
                    row["StyleId"].ToString(),
                    row["Title"].ToString(),
                    row["BuyPrice"].ToString(),
                    row["Price"].ToString(),
                    row["OrderNo"].ToString().Length > 0 ? "�ǸſϷ�" : "���� ��",
                    row["AdjustPrice"].ToString(),
                    row["Profit"].ToString(),
                };
                ListViewItem item = new ListViewItem(element);
                item.Tag = row["ListingID"].ToString();
                materialListView1.Items.Add(item);
            }

            #endregion
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
            string url = $"{API.GetServer()}/api/stocks/export/{common.StockgazersUserID}";
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

                MaterialSnackBar snackBar = new("���α׷� ��ο� �ٿ�ε� �Ǿ����", "OK", true);
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
                byte[] bytes = File.ReadAllBytes(filepath);
                sendData.Add(new StreamContent(new MemoryStream(bytes)), "csvfile", "data.csv");

                var response = await common.session.PostAsync(API.GetServer() + "/api/stocks/import", sendData);
                try
                {
                    response.EnsureSuccessStatusCode();
                    MaterialSnackBar snackBar2 = new("���ſ��� �����Ͱ� ���ε� �Ǿ����", "OK", true);
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
                materialListView1.Items.Clear();

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
                }

                foreach (var row in StockgazersReference)
                {
                    string[] element = new[] {
                        "",
                        row["StyleId"].ToString(),
                        row["Title"].ToString(),
                        row["BuyPrice"].ToString(),
                        row["Price"].ToString(),
                        row["OrderNo"].ToString().Length > 0 ? "�ǸſϷ�" : "���� ��",
                        row["AdjustPrice"].ToString(),
                        row["Profit"].ToString(),
                    };
                    ListViewItem item = new ListViewItem(element);
                    item.Tag = row["ListingID"].ToString();
                    materialListView1.Items.Add(item);
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
                MaterialSnackBar snackBar = new($"��� �߰��� �Ϸ�Ǿ����", "OK", true);
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
                    contextMenuStrip1.Show(materialListView1, e.Location);
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ModifyStockForm modifyStockForm = new ModifyStockForm(common);
            modifyStockForm.ShowDialog();
        }

        private async void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string message = $"{contextMenuStrip1.Items[0].Text} ���� ���� ����� �����ұ��?";
            if (MessageBox.Show(message, "���� ��� ����", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
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
                    return;
                }
                #endregion

                #region Stockgazers DB ������Ʈ
                url = $"{API.GetServer()}/api/stocks/{ListingID}";
                response = await common.session.DeleteAsync(url);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
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
    }
}
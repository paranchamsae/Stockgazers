using System.ComponentModel;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using Stockgazers.APIs;
using Stockgazers.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602

namespace Stockgazers
{
    public partial class AddStockForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        Common common;
        string selectedProductID = string.Empty;

        public AddStockForm(Common c)
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

        private async void materialTextBoxEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            // 딱스에서 모델정보 불러옴
            if (e.KeyCode == Keys.Enter)
            {
            retry:
                if (listView1.Items.Count > 0)
                    listView1.Items.Clear();

                string url = $"https://api.stockx.com/v2/catalog/search?query={materialTextBoxEdit1.Text.Trim().ToLower()}&pageNumber=1&pageSize=30";
                var response = await common.session.GetAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();

                    if (listView1.Items.Count > 0)
                        listView1.Items.Clear();

                    var t = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<JToken>(t);
                    var products = JsonConvert.DeserializeObject<JToken>(data["products"].ToString());

                    if (Convert.ToInt32(data["count"]) == 0)
                    {
                        MaterialSnackBar snackBar = new($"검색된 결과가 없어요", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                    else
                    {
                        // 리스트박스에 아이템 채우고 유저가 더블클릭해서 선택하게 함
                        foreach (var element in products)
                        {
                            string[] row = new[] { element["title"].ToString() };

                            ListViewItem item = new ListViewItem(row);
                            item.Tag = element["productId"].ToString();
                            item.ToolTipText = element["styleId"].ToString();
                            listView1.Items.Add(item);
                        }
                        listView1.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        if (await API.RefreshToken(common))
                            goto retry;
                        else
                        {
                            MaterialSnackBar snackBar = new("접근 토근 갱신에 실패하였습니다.", "OK", true);
                            snackBar.Show();
                            return;
                        }
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new($"{ex.Message}", "OK", true);
                        snackBar.Show(this);
                    }
                }
            }
        }

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            string message = $@"아래의 정보로 재고 추가 및 StockX에 판매 입찰이 등록됩니다.

                모델명: {materialTextBoxEdit2.Text}
                사이즈: {materialComboBox1.Text},
                구매원가: {materialTextBoxEdit4.Text} KRW,
                구매당시환율: {materialTextBoxEdit7.Text} USD,
                입찰가: {materialTextBoxEdit6.Text} USD,
                입찰하한가: {materialTextBoxEdit5.Text} USD

이 작업은 취소할 수 없습니다. 계속 할까요?";

            if (MessageBox.Show(message, "재고추가", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
            retry:
                string url = "https://api.stockx.com/v2/selling/listings";
                CreateListing createListing = new()
                {
                    amount = materialTextBoxEdit6.Text,
                    variantId = materialComboBox1.SelectedValue.ToString()
                };
                var sendData = new StringContent(JsonConvert.SerializeObject(createListing), Encoding.UTF8, "application/json");
                var response = await common.session.PostAsync(url, sendData);

                string createdListingID = string.Empty;
                try
                {
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    JObject resultJson = JsonConvert.DeserializeObject<JObject>(result);
                    createdListingID = resultJson["listingId"].ToString();
                }
                catch (Exception)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        if (await API.RefreshToken(common))
                            goto retry;
                        else
                        {
                            MaterialSnackBar snackBar = new("접근 토근 갱신에 실패하였습니다.", "OK", true);
                            snackBar.Show();
                            return;
                        }
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new("재고 추가에 실패했습니다.", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }

                if (createdListingID.Length > 0)
                {
                    try
                    {
                        Stock newStock = new()
                        {
                            UserID = common.StockgazersUserID,
                            IsDelete = "F",
                            ListingID = createdListingID,
                            StyleID = materialTextBoxEdit2.Text,
                            ProductID = selectedProductID,
                            Title = materialTextBoxEdit3.Text,
                            VariantID = materialComboBox1.SelectedValue.ToString(),
                            VariantValue = materialComboBox1.Text,
                            BuyPrice = Convert.ToInt32(materialTextBoxEdit4.Text.ToString()),
                            BuyPriceRatio = (float)Convert.ToDouble(materialTextBoxEdit7.Text),
                            BuyPriceUSD = (float)Convert.ToDouble(materialTextBoxEdit8.Text),
                            Price = Convert.ToInt32(materialTextBoxEdit6.Text.ToString()),
                            Limit = Convert.ToInt32(materialTextBoxEdit5.Text.ToString()),
                            OrderNo = string.Empty,
                            SellDatetime = null,
                            SendDatetime = null,
                            AdjustPrice = 0.0f,
                            Profit = 0.0f
                        };
                        List<Stock> stockArray = new List<Stock>() { newStock };
                        url = $"{API.GetServer()}/api/stocks";
                        sendData = new StringContent(JsonConvert.SerializeObject(stockArray), Encoding.UTF8, "application/json");
                        response = await common.session.PostAsync(url, sendData);

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        MaterialSnackBar snackBar = new("재고 추가에 실패했습니다.", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }

                MainForm.isNewStockCreated = true;
                this.Close();
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
            retry:
                string url = $"https://api.stockx.com/v2/catalog/products/{item.Tag}/variants";
                var response = await common.session.GetAsync(url);

                try
                {
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<JToken>(result);

                    if (materialComboBox1.Items.Count > 0)
                        materialComboBox1.DataSource = null;

                    BindingList<KeyValuePair<string, string>> elements = new BindingList<KeyValuePair<string, string>>();
                    foreach (var token in data)
                    {
                        elements.Add(new KeyValuePair<string, string>(token["variantValue"].ToString(), token["variantId"].ToString()));
                    }
                    materialComboBox1.DataSource = elements;
                    materialComboBox1.DisplayMember = "Key";
                    materialComboBox1.ValueMember = "Value";
                    materialComboBox1.BeginInvoke(new Action(() =>
                    {
                        materialComboBox1.SelectedIndex = -1;
                    }));

                    materialTextBoxEdit2.Text = item.ToolTipText;
                    materialTextBoxEdit3.Text = item.Text;
                    selectedProductID = item.Tag.ToString();
                }
                catch (Exception ex)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        if (await API.RefreshToken(common))
                            goto retry;
                        else
                        {
                            MaterialSnackBar snackBar = new("접근 토근 갱신에 실패하였습니다.", "OK", true);
                            snackBar.Show(this);
                            return;
                        }
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new($"{ex.Message}", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }
            }
        }

        private async void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedIndex == -1 || selectedProductID.Length == 0)
                return;

            string VariantID = materialComboBox1.SelectedValue.ToString();

        retry:
            string url = $"https://api.stockx.com/v2/catalog/products/{selectedProductID}/variants/{VariantID}/market-data";
            var response = await common.session.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<JToken>(result);

                button1.Text = "더 많은 수익\n" + data["earnMoreAmount"].ToString() + " USD";
                button1.Tag = data["earnMoreAmount"].ToString();
                button2.Text = "더 빨리 판매하기\n" + data["sellFasterAmount"].ToString() + " USD";
                button2.Tag = data["sellFasterAmount"].ToString();
                button3.Text = "즉시 판매하기\n" + data["highestBidAmount"].ToString() + " USD";
                button3.Tag = data["highestBidAmount"].ToString();
            }
            catch (Exception ex)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (await API.RefreshToken(common))
                        goto retry;
                    else
                    {
                        MaterialSnackBar snackBar = new("접근 토근 갱신에 실패하였습니다.", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }
                else
                {
                    MaterialSnackBar snackBar = new($"{ex.Message}", "OK", true);
                    snackBar.Show(this);
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;
            button1.BackColor = Color.Black;

            button2.ForeColor = Color.Black;
            button2.BackColor = Color.White;
            button3.ForeColor = Color.Black;
            button3.BackColor = Color.White;

            materialTextBoxEdit6.Text = button1.Tag.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;
            button2.BackColor = Color.Black;

            button1.ForeColor = Color.Black;
            button1.BackColor = Color.White;
            button3.ForeColor = Color.Black;
            button3.BackColor = Color.White;

            materialTextBoxEdit6.Text = button2.Tag.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.ForeColor = Color.White;
            button3.BackColor = Color.Black;

            button1.ForeColor = Color.Black;
            button1.BackColor = Color.White;
            button2.ForeColor = Color.Black;
            button2.BackColor = Color.White;

            materialTextBoxEdit6.Text = button3.Tag.ToString();
        }

        private async void AddStockForm_Load(object sender, EventArgs e)
        {
            string url = $"{API.GetServer()}/api/stocks/ratio";
            var response = await common.session.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                materialTextBoxEdit7.Text = result;
            }
            catch (Exception ex)
            {
                materialTextBoxEdit7.Text = "";
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
            }
        }

        /// <summary>
        /// 구매원가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialTextBoxEdit4_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex("^[0-9]+(.)?[0-9]{1,2}$");
            if (materialTextBoxEdit7.Text.Length > 0 && materialTextBoxEdit4.Text.Length > 0 && regex.IsMatch(materialTextBoxEdit7.Text) && regex.IsMatch(materialTextBoxEdit4.Text))
            {
                var buyPriceUSD = Convert.ToInt32(materialTextBoxEdit4.Text) / Convert.ToDouble(materialTextBoxEdit7.Text);
                materialTextBoxEdit8.Text = Math.Round(buyPriceUSD, 2).ToString();
            }
        }

        /// <summary>
        /// 환율
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialTextBoxEdit7_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex("^[0-9]+(.)?[0-9]{1,2}$");
            if (materialTextBoxEdit4.Text.Length > 0 && materialTextBoxEdit7.Text.Length > 0 && regex.IsMatch(materialTextBoxEdit4.Text) && regex.IsMatch(materialTextBoxEdit7.Text))
            {
                var buyPriceUSD = Convert.ToInt32(materialTextBoxEdit4.Text) / Convert.ToDouble(materialTextBoxEdit7.Text);
                materialTextBoxEdit8.Text = Math.Round(buyPriceUSD, 2).ToString();
            }
        }
    }
}

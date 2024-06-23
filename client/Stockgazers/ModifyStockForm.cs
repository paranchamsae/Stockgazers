using System.ComponentModel;
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

#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602

namespace Stockgazers
{
    public partial class ModifyStockForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        Common common;
        //string selectedProductID = string.Empty;
        string ListingID = string.Empty;

        public ModifyStockForm(Common c, string listingID)
        {
            InitializeComponent();
            common = c;
            ListingID = listingID;

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

        private async void ModifyStockForm_Load(object sender, EventArgs e)
        {
            string url = $"{API.GetServer()}/api/listing/{ListingID}";
            var response = await common.session.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<JToken>(result);

                materialTextBoxEdit2.Text = data[0]["StyleID"].ToString();
                materialTextBoxEdit3.Text = data[0]["Title"].ToString();
                materialTextBoxEdit1.Text = data[0]["VariantValue"].ToString();
                materialTextBoxEdit4.Text = data[0]["BuyPrice"].ToString();
                materialTextBoxEdit6.Text = data[0]["Price"].ToString();
                materialTextBoxEdit5.Text = data[0]["Limit"].ToString();
                materialTextBoxEdit7.Text = data[0]["BuyPriceRatio"].ToString();
                materialTextBoxEdit8.Text = data[0]["BuyPriceUSD"].ToString();

            retry:
                url = $"https://api.stockx.com/v2/catalog/products/{data[0]["ProductID"].ToString()}/variants/{data[0]["VariantID"]}/market-data";
                response = await common.session.GetAsync(url);
                try
                {
                    response.EnsureSuccessStatusCode();

                    result = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<JToken>(result);

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
                            snackBar.Show();
                            return;
                        }
                    }
                    else
                    {
                        MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                        snackBar.Show();
                        return;
                    }
                }
            }
            catch (Exception)
            {
                MaterialSnackBar snackBar = new($"서버와의 통신에 실패했어요 :(", "OK", true);
                snackBar.Show(this);
            }
        }

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            string message = $@"아래의 정보로 입찰 정보가 수정됩니다.

    모델명: {materialTextBoxEdit2.Text}
    사이즈: {materialTextBoxEdit1.Text},

    구매원가: {materialTextBoxEdit4.Text} KRW,
    적용환율: {materialTextBoxEdit7.Text},
    입찰가: {materialTextBoxEdit6.Text} USD,
    입찰하한제한: {materialTextBoxEdit5.Text} USD

이 작업은 취소할 수 없습니다. 계속 할까요?";

            if (MessageBox.Show(message, "입찰수정", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
            retry:
                string url = $"https://api.stockx.com/v2/selling/listings/{ListingID}";
                Dictionary<string, string> data = new()
                {
                    { "amount", materialTextBoxEdit6.Text },
                    { "currencyCode", "USD" },
                    { "expiresAt", DateTime.UtcNow.AddMonths(1).ToString("O") }
                };
                var sendData = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await common.session.PatchAsync(url, sendData);
                try
                {
                    response.EnsureSuccessStatusCode();
                    url = $"{API.GetServer()}/api/listing";
                    RequestPatchListing data2 = new()
                    {
                        ListingID = ListingID,
                        BuyPrice = Convert.ToInt32(materialTextBoxEdit4.Text),
                        BuyPriceRatio = Math.Round(Convert.ToDouble(materialTextBoxEdit7.Text), 2),
                        BuyPriceUSD = Math.Round(Convert.ToDouble(materialTextBoxEdit8.Text), 2),
                        Price = Convert.ToInt32(materialTextBoxEdit6.Text),
                        Limit = Convert.ToInt32(materialTextBoxEdit5.Text),
                        Status = "ACTIVE"
                    };
                    sendData = new StringContent(JsonConvert.SerializeObject(data2), Encoding.UTF8, "application/json");
                    response = await common.session.PatchAsync(url, sendData);
                    response.EnsureSuccessStatusCode();

                    MainForm.isNewStockCreated = true;
                    this.Close();
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
                        MaterialSnackBar snackBar = new($"서버와의 통신에 실패했어요 :(", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                }
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
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

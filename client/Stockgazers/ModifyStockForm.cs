using System.ComponentModel;
using System.Text;
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
        string selectedProductID = string.Empty;

        public ModifyStockForm(Common c)
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

       

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            string message = $@"아래의 정보로 재고 추가 및 StockX에 판매 입찰이 등록됩니다.

                모델명: {materialTextBoxEdit2.Text}
                사이즈: { materialComboBox1.Text },
                구매원가: {materialTextBoxEdit4.Text} KRW,
                입찰가: {materialTextBoxEdit6.Text} USD,
                입찰하한제한: {materialTextBoxEdit5.Text} USD

이 작업은 취소할 수 없습니다. 계속 할까요?";

            if (MessageBox.Show(message, "재고추가", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
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
                    MaterialSnackBar snackBar = new("재고 추가에 실패했습니다.", "OK", true);
                    snackBar.Show(this);
                    return;
                }

                if (createdListingID.Length > 0)
                {
                    try
                    {
                        int buyprice = Convert.ToInt32(materialTextBoxEdit4.Text.ToString());
                        float buypriceUsd = (float)buyprice / 1300;
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
                            BuyPrice = buyprice,
                            BuyPriceUSD = (float)Math.Round(buypriceUsd, 2),
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

       
    }
}

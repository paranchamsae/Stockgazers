using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReaLTaiizor.Child.Material;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Stockgazers
{
    public partial class AddStockForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        Common common;

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
                if (listView1.Items.Count > 0)
                    listView1.Items.Clear();

                string url = $"https://api.stockx.com/v2/catalog/search?query={materialTextBoxEdit1.Text.Trim().ToLower()}&pageNumber=1&pageSize=30";

                try
                {
                    var response = await common.session.GetAsync(url);
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
                    MaterialSnackBar snackBar = new($"{ex.Message}", "OK", true);
                    snackBar.Show(this);
                }
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
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
                    foreach(var token in data)
                    {
                        elements.Add(new KeyValuePair<string, string>(token["variantValue"].ToString(), token["variantId"].ToString() ));
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
                }
                catch (Exception ex)
                {
                    MaterialSnackBar snackBar = new($"{ex.Message}", "OK", true);
                    snackBar.Show(this);
                }
            }
        }
    }
}

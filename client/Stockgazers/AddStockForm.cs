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
                if (materialListView1.Items.Count > 0)
                    materialListView1.Items.Clear();

                string url = $"https://api.stockx.com/v2/catalog/search?query={materialTextBoxEdit1.Text.Trim().ToLower()}&pageNumber=1&pageSize=1";

                try
                {
                    var response = await common.session.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var t = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<JToken>(t);
                    var products = JsonConvert.DeserializeObject<JToken>(data["products"].ToString());

                    if (Convert.ToInt32(data["count"]) < 1)
                    {
                        MaterialSnackBar snackBar = new($"검색된 결과가 없어요", "OK", true);
                        snackBar.Show(this);
                        return;
                    }
                    else if (Convert.ToInt32(data["count"]) == 1)
                    {
                        // 아래 텍스트박스에 자동 입력
                        foreach (var element in products)
                        {
                            string[] row = new[] { element["title"].ToString() };

                            ListViewItem item = new ListViewItem(row);
                            item.Tag = element["styleId"].ToString();
                            materialListView1.Items.Add(item);
                        }
                        materialListView1.Invalidate();
                    }
                    else
                    {
                        // 리스트박스에 아이템 채우고 유저가 더블클릭해서 선택하게 함
                        materialListView1.Invalidate();
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
            this.Close();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = materialListView1.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                materialTextBoxEdit2.Text = item.Tag.ToString();
                materialTextBoxEdit3.Text = item.Text;
            }
        }
    }
}

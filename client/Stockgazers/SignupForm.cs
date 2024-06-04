using Newtonsoft.Json;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using Stockgazers.APIs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Stockgazers
{
    public partial class SignupForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        Common common;

        public SignupForm(Common c)
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
            // 입력되지 않은 항목이 있는지 검사
            if(materialTextBoxEdit1.Text.Length == 0 || materialTextBoxEdit2.Text.Length == 0 || materialTextBoxEdit3.Text.Length == 0 || materialTextBoxEdit4.Text.Length == 0)
            {
                MaterialSnackBar snackBar = new("입력되지 않은 항목이 있어요", "OK", true);
                snackBar.Show(this);
                return;
            }

            // 클라이언트단에서는 비밀번호/비밀번호확인 일치 여부, 올바른 이메일 형식인지 까지만 확인해주면 됨
            if (materialTextBoxEdit2.Text != materialTextBoxEdit3.Text)
            {
                // 비밀번호 확인 일치하지 않음
                MaterialSnackBar snackBar = new("비밀번호가 일치하지 않아요", "OK", true);
                snackBar.Show(this);
                return;
            }

            Regex regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{3}$");
            if (!regex.IsMatch(materialTextBoxEdit4.Text))
            {
                // 올바른 이메일 형식이 아님
                MaterialSnackBar snackBar = new("올바른 이메일 형식이 아니에요", "OK", true);
                snackBar.Show(this);
                return;
            }

            // 아이디 중복검사는 서버에 API 요청시 검증 가능 > 중복 아이디가 있을때 http 409를 리턴함
            string url = $"{API.GetServer()}/api/user";
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "", "" }
            };
            var sendData = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await common.session.PostAsync(url, sendData);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MaterialSnackBar snackBar = new(ex.Message, "OK", true);
                snackBar.Show(this);
            }
        }
    }
}

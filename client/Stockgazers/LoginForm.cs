using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stockgazers.APIs;
using System.Text;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;

#pragma warning disable CS8601
namespace Stockgazers
{
    public partial class LoginForm : MaterialForm
    {
        Common common;
        public static bool isSignedUp = false;

        class Login
        {
            public required string ID { get; set; }
            public required string PW { get; set; }
        }

        public LoginForm(Common c)
        {
            InitializeComponent();
            common = c;
        }

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            Login data = new()
            {
                ID = materialTextBoxEdit1.Text,
                PW = materialTextBoxEdit2.Text
            };

            string url = $"{API.GetServer()}/api/user/login";
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                                  "application/json");

            try
            {
                var response = await common.session.PostAsync(url, content);
                string result = response.Content.ReadAsStringAsync().Result;
                JObject? resultjson = JsonConvert.DeserializeObject<JObject>(result);
                if (resultjson == null)
                {
                    MaterialSnackBar snackBar = new("서버와의 통신에 실패하였습니다.\r\n잠시 후 다시 시도 해 주세요.", "OK", true);
                    snackBar.Show(this);
                    return;
                }
                else if (resultjson != null && resultjson.Value<string>("statusCode") != "200")
                {
                    MaterialSnackBar snackBar = new("삭제되었거나 아이디/패스워드 정보가 다른 계정입니다.", "OK", true);
                    snackBar.Show(this);
                    return;
                }
                else if (resultjson != null && resultjson.Value<string>("statusCode") == "200")
                {
                    JToken? userData = resultjson["data"];
                    if(userData != null)
                    {
                        common.StockgazersUserID = userData.Value<int>("ID");
                        common.UserTier = userData.Value<int>("Tier");
                        common.DiscountType = userData.Value<string>("DiscountType");
                    }

                    JToken? core = resultjson["core"];

                    API.APIKey = core?.Value<string>("Key1");
                    API.ClientID = core?.Value<string>("Key2");
                    API.ClientSecret = core?.Value<string>("Key3");

                    if (API.APIKey == null || API.ClientID == null || API.ClientSecret == null)
                    {
                        MaterialSnackBar snackBar = new("서버와의 통신에 실패하였습니다.\r\n잠시 후 다시 시도 해 주세요.", "OK", true);
                        snackBar.Show(this);
                        API.APIKey = null;
                        API.ClientID = null;
                        API.ClientSecret = null;
                        return;
                    }
                    common.IsAppLogin = true;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message.Split(new char[] { '(' }).First().Trim() + "\r\n잠시 후 다시 시도 해 주세요.", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            SignupForm signupForm = new SignupForm(common);
            signupForm.ShowDialog();

            if(isSignedUp)
            {
                isSignedUp = false;
                MaterialSnackBar snackBar = new("계정 생성이 완료되었어요!", "OK", true);
                snackBar.Show(this);
            }
        }

        private void materialTextBoxEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                materialButton1_Click(this, e);
        }

        private void materialTextBoxEdit2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                materialButton1_Click(this, e);
        }
    }
}

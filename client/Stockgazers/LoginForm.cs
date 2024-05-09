using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stockgazers.APIs;
using System.Text;
using ReaLTaiizor.Forms;

namespace Stockgazers
{
    public partial class LoginForm : MaterialForm
    {
        Common common;

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

        private async void button1_Click(object sender, EventArgs e)
        {
            Login data = new()
            {
                ID = textBox_id.Text,
                PW = textBox_passwd.Text
            };

            string url = $"{API.GetServer()}/api/user/login";
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                                  "application/json");

            var response = await common.session.PostAsync(url, content);

            string result = response.Content.ReadAsStringAsync().Result;
            var resultjson = JsonConvert.DeserializeObject<JObject>(result);
            if (resultjson == null)
            {
                MessageBox.Show("서버와의 통신에 실패하였습니다.\r\n잠시 후 다시 시도 해 주세요.", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (resultjson != null && resultjson.Value<string>("statusCode") != "200")
            {
                MessageBox.Show("삭제되었거나 아이디/패스워드 정보가 다른 계정입니다.", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (resultjson != null && resultjson.Value<string>("statusCode") == "200")
            {
                JToken? core = resultjson["core"];

                API.APIKey = core?.Value<string>("Key1");
                API.ClientID = core?.Value<string>("Key2");
                API.ClientSecret = core?.Value<string>("Key3");

                if (API.APIKey == null || API.ClientID == null || API.ClientSecret == null)
                {
                    MessageBox.Show("서버와의 통신에 실패하였습니다.\r\n잠시 후 다시 시도 해 주세요.", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    API.APIKey = null;
                    API.ClientID = null;
                    API.ClientSecret = null;
                    return;
                }
                common.IsAppLogin = true;
                this.Close();
            }
        }
    }
}

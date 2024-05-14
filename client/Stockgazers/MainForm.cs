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

namespace Stockgazers
{
    public partial class MainForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

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

        class Token
        {
            public required string grant_type { get; set; }
            public required string client_id { get; set; }
            public required string client_secret { get; set; }
            public required string code { get; set; }
            public required string redirect_uri { get; set; }
        }

        Common common;

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
            #region 1. ���� �α��� �ϰ� ���� �ڵ带 ȹ����
            //string param = $@"response_type=code&client_id={API.ClientID}&redirect_uri={API.GetCallback()}&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            string param = $@"response_type=code&client_id={API.ClientID}&redirect_uri=https://stockgazers.kr/api/callback&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            StockXLoginForm stockXLoginForm = new(this, param);
            stockXLoginForm.ShowDialog();

            if (AuthCode == string.Empty || AuthCode.Length <= 0)       // ���� ����(���� �ڵ尡 ����)
            {
                MessageBox.Show("���� �ڵ� ȹ�� ����, ���� ��α��� �ʿ�", "Stockgazers", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region 2. ȹ���� �����ڵ�� ���� �׼���/�������� ��ū�� ����
            if (API.ClientID == null || API.ClientSecret == null)
                return;

            string url = $"https://accounts.stockx.com/oauth/token";
            Token data = new()
            {
                grant_type = "authorization_code",
                client_id = API.ClientID,
                client_secret = API.ClientSecret,
                code = AuthCode,
                redirect_uri = "https://stockgazers.kr/api/callback"
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
            // ���� ��� ����� �������� ������ ��

            // ���������� �� ��û�� ������...
            // ��ü �����͸� �������� �޾ƿ��� ���� �� > �ٸ��κ� ������ ������Ʈ �ϰ� ����
            // ������/��/�Ϸ� �Ǽ��� ��û ���ٸ�??   ... ���� ����� �ؾ��� �� ����

            #region 2-1. ������ ��ϵ� �� ��ü �Ǹ���Ȳ
            url = $"https://api.stockx.com/v2/selling/listings";
            common.session.DefaultRequestHeaders.Add("Authorization", $"Bearer {common.AccessToken}");
            common.session.DefaultRequestHeaders.Add("x-api-key", $"{API.APIKey}");

            response = await common.session.GetAsync(url);
            List<JToken> StockxListingsList = new List<JToken>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var StockxRaw = response.Content.ReadAsStringAsync().Result;
                JToken? tempStockx = JsonConvert.DeserializeObject<JToken>(StockxRaw);
                if (tempStockx != null)
                    StockxListingsList = JsonConvert.DeserializeObject<JToken>(tempStockx["listings"]!.ToString())!.ToList();
            }
            if (StockxListingsList.Count == 0)
            {
                MaterialSnackBar snackBar = new("StockX���� ���� �Ǹ� ��Ȳ�� �ҷ����µ� �����߽��ϴ�.", "OK", true);
                snackBar.Show(this);
                return;
            }

            #endregion

            #region 2-2. Stockgazers DB ������ ���� ���� �� ��ü �Ǹ� ��Ȳ
            url = $"http://127.0.0.1:8000/api/stocks/{common.StockgazersUserID}";
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

            #region 2-3. ���� ����̶� Stockgazers �Ǹ���Ȳ Distinct�� �Ÿ��� �����ִ� �����ʹ� ��� �߰�

            #endregion

            #endregion
        }
    }
}
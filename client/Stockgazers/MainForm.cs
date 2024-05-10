using System.Net;
using Stockgazers.APIs;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Util;

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

        //API server;
        //HttpClient session;
        Common common;

        public MainForm()
        {
            //server = new();
            //session = new HttpClient();

            InitializeComponent();

            common = new();
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

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"authcode: {AuthCode}, state: {state}");
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

            #region 2. �� ��� ����� �������� ������ �ͼ� Ŭ���̾�Ʈ�� �ѷ���
            // ���� ��� ����� �������� ������ ��

            // ���������� �� ��û�� ������...
            // ��ü �����͸� �������� �޾ƿ��� ���� �� > �ٸ��κ� ������ ������Ʈ �ϰ� ����
            // ������/��/�Ϸ� �Ǽ��� ��û ���ٸ�??   ... ���� ����� �ؾ��� �� ����
            HttpResponseMessage result = await common.server.Call(common.session, HttpMethod.Get, "https://stockgazers.kr/");
            if (result.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("error");

            // ��� ��������� Ŭ���̾�Ʈ �信 ������ ���ε�
            #endregion
        }
    }
}
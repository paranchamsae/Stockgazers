using System.Net;
using Stockgazers.APIs;

namespace Stockgazers
{
    public partial class MainForm : Form
    {
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

        API server;
        HttpClient session;

        public MainForm()
        {
            server = new();
            session = new HttpClient();

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"authcode: {AuthCode}, state: {state}");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            #region 1. ���� �α��� �ϰ� ���� �ڵ带 ȹ����
            string param = $@"response_type=code&client_id={API.GetID()}&redirect_uri={API.GetCallback()}&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            StockXLoginForm stockXLoginForm = new(this, param);
            stockXLoginForm.ShowDialog();

            if (AuthCode == string.Empty || AuthCode.Length <= 0)       // ���� ����(���� �ڵ尡 ����)
                MessageBox.Show("error");
            #endregion

            #region 2. �� ��� ����� �������� ������ �ͼ� Ŭ���̾�Ʈ�� �ѷ���
            // ���� ��� ����� �������� ������ ��

            // ���������� �� ��û�� ������...
            // ��ü �����͸� �������� �޾ƿ��� ���� �� > �ٸ��κ� ������ ������Ʈ �ϰ� ����
            // ������/��/�Ϸ� �Ǽ��� ��û ���ٸ�??   ... ���� ����� �ؾ��� �� ����
            HttpResponseMessage result = await server.Call(session, HttpMethod.Get, "https://stockgazers.kr/");
            if (result.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("error");

            // ��� ��������� Ŭ���̾�Ʈ �信 ������ ���ε�
            #endregion
        }
    }
}
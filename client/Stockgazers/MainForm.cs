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
            #region 1. 딱스 로그인 하고 인증 코드를 획득함
            string param = $@"response_type=code&client_id={API.GetID()}&redirect_uri={API.GetCallback()}&scope=offline_access%20openid&audience=gateway.stockx.com&state=abcXYZ0987";
            StockXLoginForm stockXLoginForm = new(this, param);
            stockXLoginForm.ShowDialog();

            if (AuthCode == string.Empty || AuthCode.Length <= 0)       // 인증 실패(인증 코드가 없음)
                MessageBox.Show("error");
            #endregion

            #region 2. 내 재고 목록을 서버에서 가지고 와서 클라이언트에 뿌려줌
            // 나의 재고 목록을 서버에서 가지고 옴

            // 서버에서는 이 요청을 받으면...
            // 전체 데이터를 딱스에서 받아오고 디비랑 비교 > 다른부분 있으면 업데이트 하고 리턴
            // 진행전/중/완료 건수가 엄청 많다면??   ... 로직 고민을 해야할 것 같다
            HttpResponseMessage result = await server.Call(session, HttpMethod.Get, "https://stockgazers.kr/");
            if (result.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("error");

            // 목록 가지고오면 클라이언트 뷰에 데이터 바인딩
            #endregion
        }
    }
}
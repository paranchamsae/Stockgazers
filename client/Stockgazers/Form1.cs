using Stockgazers.APIs;

namespace Stockgazers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            HttpClient client = new HttpClient();
            API api = new();

            //api.Call(client, "");
            
            InitializeComponent();
        }
    }
}
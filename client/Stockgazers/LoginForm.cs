using Newtonsoft.Json;
using Stockgazers.APIs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stockgazers
{
    public partial class LoginForm : Form
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

            string url = "https://stockgazers.kr/api/user/login";
            var content = new StringContent(JsonConvert.SerializeObject(data), UnicodeEncoding.UTF8,
                                  "application/x-www-form-urlencoded");

            //var response = await common.session.PostAsync(url, content);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    //response.
            //    //MessageBox.Show("respon");
            //    return;
            //}

            common.IsAppLogin = true;
            this.Close();
        }
    }
}

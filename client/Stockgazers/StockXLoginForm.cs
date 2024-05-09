using Microsoft.Web.WebView2.WinForms;
using Stockgazers.APIs;
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

namespace Stockgazers
{
    public partial class StockXLoginForm : Form
    {
        MainForm f;

        public StockXLoginForm(MainForm form, string param)
        {
            InitializeComponent();

            f = form;
            webView.Source = new Uri($"{API.GetAuth()}?{param}");
            webView.SourceChanged += WebView_SourceChanged;
        }

        private void WebView_SourceChanged(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            Trace.WriteLine(webView.Source.ToString());
            // 딱스 로그인이 살아있거나 새로 로그인이 성공하여 콜백을 받음
            if (webView.Source.ToString().StartsWith("https://stockgazers.kr/api/callback"))
                BackToMainform();
        }

        private void BackToMainform()
        {
            f.AuthCode = webView.Source.ToString().Split("=")[1].Split("&").First();
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace VRCamRipper
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            this.webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            InitializeAsync();
        }

        private async void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webView.CoreWebView2.GetDevToolsProtocolEventReceiver("Network.webSocketFrameReceived").DevToolsProtocolEventReceived += OnConsoleMessage;
            await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.enable", "{ }");

            this.webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Linux; Android 10; Quest 2) AppleWebKit/537.36 (KHTML, like Gecko) OculusBrowser/13.0.0.2.16.259832224 SamsungBrowser/4.0 Chrome/87.0.4280.66 VR Safari/537.36";
        }

        private async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        }

        private void btnNavigate_Click(object sender, EventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                try
                {
                    webView.CoreWebView2.Navigate(txtAddress.Text);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }

        private void OnConsoleMessage(object sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs e)
        {
            if (e != null && e.ParameterObjectAsJson != null)
            {
                try
                {
                    var payload = JsonConvert.DeserializeObject<Payload>(e.ParameterObjectAsJson);

                    string name = txtAddress.Text.Split('/').Last();

                    if (!Directory.Exists(Path.Combine(name, payload.requestId)))
                    {
                        Directory.CreateDirectory(Path.Combine(name, payload.requestId));
                    }

                    byte[] data = Convert.FromBase64String(payload.response.payloadData);
                    File.WriteAllBytes(Path.Combine(Path.Combine(name, payload.requestId), payload.timestamp + ".dat"), data);
                }
                catch
                {

                }
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace VRCamRipper
{
    public class PayloadResponse
    {
        public string payloadData { get; set; }
    }

    public class Payload
    {
        public string requestId { get; set; }
        public PayloadResponse response { get; set; }
        public string timestamp { get; set; }
        public string data;
    }

    public partial class frmMain : Form
    {
        int count = 0;
        private WebClient _webClient;

        public frmMain()
        {
            InitializeComponent();
            _webClient = new WebClient();

            this.webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            InitializeAsync();
        }

        private async void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webView.CoreWebView2.GetDevToolsProtocolEventReceiver("Network.webSocketFrameReceived").DevToolsProtocolEventReceived += OnConsoleMessage;
            await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.enable", "{ }");

            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

            this.webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Linux; Android 10; Quest 2) AppleWebKit/537.36 (KHTML, like Gecko) OculusBrowser/16.6.0.1.52.314146309 SamsungBrowser/4.0 Chrome/91.0.4472.164 VR Safari/537.36";
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (e.Request.Uri.EndsWith(".mp4"))
            {
                string name = txtAddress.Text.Split('/').Last();

                string path = Path.Combine(name, count.ToString());

                if (count == 0)
                {
                    count += Directory.GetDirectories(name).Length - 1;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }

                if (e.Request.Uri.Contains("init"))
                {
                    count++;
                    path = Path.Combine(name, count.ToString());

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    _webClient.DownloadFile(e.Request.Uri, Path.Combine(path, "0" + e.Request.Uri.Split('/').Last()));
                }
                else
                {
                    _webClient.DownloadFile(e.Request.Uri, Path.Combine(path, e.Request.Uri.Split('/').Last()));
                }
            }
        }
        private async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async();
        }

        private void btnNavigate_Click(object sender, EventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                try
                {
                    count = 0;
                    string name = txtAddress.Text.Split('/').Last();

                    if (!Directory.Exists(name))
                    {
                        Directory.CreateDirectory(name);
                    }

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

                    if (payload == null || payload.response == null || string.IsNullOrWhiteSpace(payload.response.payloadData))
                    {
                        return;
                    }

                    byte[] data = Convert.FromBase64String(payload.response.payloadData);
                    string name = txtAddress.Text.Split('/').Last();

                    if (!Directory.Exists(Path.Combine(name, payload.requestId)))
                    {
                        Directory.CreateDirectory(Path.Combine(name, payload.requestId));
                    }

                    File.WriteAllBytes(Path.Combine(Path.Combine(name, payload.requestId), payload.timestamp + ".dat"), data);
                }
                catch
                {

                }
            }
        }
    }
}

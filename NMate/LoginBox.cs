using System.Windows.Forms;
using System.Web;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace NMate
{
    public partial class LoginBox : Form
    {

        public class EnvatoResponse
        {
            public string refresh_token { get; set; }
            public string token_type { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        // Objects //
        public EnvatoResponse ENVATO = new EnvatoResponse();









        public LoginBox()
        {
            InitializeComponent();
            // 2 - We need to navigate to the Envato Login Page //
            webWindow.Navigate("https://api.envato.com/authorization?response_type=code&client_id=nvatowin-rjj5eanv&redirect_uri=https://envato.com/index.html");
        }

        private async void webWindow_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webWindow.Url.ToString().Substring(0, 12) == "https://enva")
            {
                // 3 - We must be on a page with the code in the URL //
                string URLCode = HttpUtility.ParseQueryString(webWindow.Url.Query).Get("code");

                // 4 - Request a token from the Envato Servers //
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "grant_type", "authorization_code" },
                        { "code", URLCode },
                        { "client_id", "nvatowin-rjj5eanv" },
                        { "client_secret", "Bv8zscl0AaqtaWUdCADe1Ic1Yan4ZwS5" }
                    };
                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync("https://api.envato.com/token", content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    ENVATO = JsonConvert.DeserializeObject<EnvatoResponse>(responseString);
                }

                // We only want the refresh token - we can continually use this //
                // 5 - Save the refresh token to a file. It isn't vunerable information, it's random code //
                File.WriteAllText(@"refresh_token.nmate", ENVATO.refresh_token);

                // We're now done with the token. We can dispose of the window //
                // 6 - Fade the window out //
                while(this.Opacity != 0)
                {
                    this.Opacity -= 0.01;
                    Thread.Sleep(1);
                }

                // 7 - Dispose of the window //
                this.Dispose();
            }
        }
    }
}

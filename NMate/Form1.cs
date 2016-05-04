using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace NMate
{
    public partial class NAME : Form
    {
        // Making the window draggable //
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }
        // Making the window draggable //



        // Classes //
        public class EnvatoResponse
        {
            public string token_type = "";
            public string access_token = "";
            public int expires_in = 0;
        }

        public class Account
        {
            public string image { get; set; }
            public string firstname { get; set; }
            public string surname { get; set; }
            public string available_earnings { get; set; }
            public string total_deposits { get; set; }
            public string balance { get; set; }
            public string country { get; set; }
        }

        public class AccountRoot
        {
            public Account account { get; set; }
        }

        public class Email
        {
            public string email { get; set; }
        }

        public class Username
        {
            public string username { get; set; }
        }






        public NAME()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, System.EventArgs e)
        {
            // 0 - Set a tooltip for the "Logout" Button //
            ToolTip buttontip = new ToolTip();
            buttontip.SetToolTip(this.LogOutButton, "Log Out?");

            // 0.1 - Read in the file "refresh_token.nmate". If it's not empty, must not be first-time run //
            if (File.ReadAllText(@"refresh_token.nmate") == "")
            {
                // When the app starts up for the first time, we need to request an refresh token //
                // 1 - Open up the Login Box and make it screen-centered //
                LoginBox login = new LoginBox();
                login.StartPosition = FormStartPosition.CenterScreen;

                // 1.1 - Show the Login Screen box //
                login.ShowDialog();
            }

            // 8 - Get an access token //
            EnvatoResponse ENVATO = await getAnAccessToken();

            // 11 - Populate the UI with User Information //
            populateUIWithUserInfo(ENVATO);
        }

        public async Task<EnvatoResponse> getAnAccessToken()
        {
            // 9 - Read the file containing the "refresh_token" in //
            var refresh_token = File.ReadAllText(@"refresh_token.nmate");

            // 10 - Now request an access token from Envato's servers //
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                    {
                        { "grant_type", "refresh_token" },
                        { "refresh_token", refresh_token },
                        { "client_id", "nvatowin-rjj5eanv" },
                        { "client_secret", "Bv8zscl0AaqtaWUdCADe1Ic1Yan4ZwS5" }
                    };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("https://api.envato.com/token", content);
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EnvatoResponse>(responseString);
            }
        }

        public async void populateUIWithUserInfo(EnvatoResponse ENVATO)
        {
            // N/A - Write the access token to the file "access_token.nmate" //
            File.WriteAllText(@"access_token.nmate", ENVATO.access_token);

            using (var client = new HttpClient())
            {
                var url = "https://api.envato.com/v1/market/private/user/account.json";
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ENVATO.access_token);
                var response = await client.GetStringAsync(url);
                AccountRoot envato_account = JsonConvert.DeserializeObject<AccountRoot>(response);

                var url2 = "https://api.envato.com/v1/market/private/user/email.json";
                var response2 = await client.GetStringAsync(url2);
                Email email = JsonConvert.DeserializeObject<Email>(response2);

                var url3 = "https://api.envato.com/v1/market/private/user/username.json";
                var response3 = await client.GetStringAsync(url3);
                Username username = JsonConvert.DeserializeObject<Username>(response3);

                NameBox.Text = envato_account.account.firstname + " " + envato_account.account.surname;
                ProfilePictureBox.ImageLocation = envato_account.account.image;
                EarningsBox.Text = "Balance: $" + envato_account.account.balance;
                AvailableEarningsBox.Text = "Available Earnings: $" + envato_account.account.balance;
                CountryBox.Text = envato_account.account.country;
                TotalDepositsBox.Text = "Total Deposits: $" + envato_account.account.total_deposits;
                EmailBox.Text = "Email: " + email.email;
                UsernameBox.Text = "Username: " + username.username;
            }
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            // N/A - Wipe the "refresh_token.nmate" file, and close the program //
            File.WriteAllText(@"refresh_token.nmate", "");
            this.Dispose();
        }

        private void themeforest_Click(object sender, EventArgs e)
        {
            ThemeForest ThemeForestBox = new ThemeForest();
            ThemeForestBox.ShowDialog();

        }
    }
}

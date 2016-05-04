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
using System.Diagnostics;

namespace NMate
{
    public partial class ThemeForest : Form
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
        public class FeaturedFile
        {
            public string id { get; set; }
            public string item { get; set; }
            public string url { get; set; }
            public string user { get; set; }
            public string thumbnail { get; set; }
            public string sales { get; set; }
            public string rating { get; set; }
            public string rating_decimal { get; set; }
            public string cost { get; set; }
            public string uploaded_on { get; set; }
            public string last_update { get; set; }
            public string tags { get; set; }
            public string category { get; set; }
            public string live_preview_url { get; set; }
        }

        public class FeaturedAuthor
        {
            public string id { get; set; }
            public string user { get; set; }
            public string url { get; set; }
            public string thumbnail { get; set; }
        }

        public class FreeFile
        {
            public string id { get; set; }
            public string item { get; set; }
            public string url { get; set; }
            public string user { get; set; }
            public string thumbnail { get; set; }
            public string sales { get; set; }
            public string rating { get; set; }
            public string rating_decimal { get; set; }
            public string cost { get; set; }
            public string uploaded_on { get; set; }
            public string last_update { get; set; }
            public string tags { get; set; }
            public string category { get; set; }
            public string live_preview_url { get; set; }
        }

        public class Features
        {
            public FeaturedFile featured_file { get; set; }
            public FeaturedAuthor featured_author { get; set; }
            public FreeFile free_file { get; set; }
        }

        public class RootObject
        {
            public Features features { get; set; }
        }
        // Classes //













        public ThemeForest()
        {
            InitializeComponent();
        }

        private void ThemeForest_Load(object sender, EventArgs e)
        {
            // 1 - Load in the "access_token.nmate" file //
            string access_token = System.IO.File.ReadAllText(@"access_token.nmate");

            // 2 - Get the featured items/people/freebies for ThemeForest //
            getFeaturedInfo(access_token);
        }

        private async void getFeaturedInfo(string access_token)
        {
            string url = "https://api.envato.com/v1/market/features:themeforest.json";
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();
            var RESPONSE = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
                RESPONSE = await client.GetStringAsync(url);
            }
            RootObject featured = JsonConvert.DeserializeObject<RootObject>(RESPONSE);

            // Featured Author //
            AuthorBox.ImageLocation = featured.features.featured_author.thumbnail;
            NameBox.Text = featured.features.featured_author.user;
            ToolTip NameBoxToolTip = new ToolTip();
            NameBoxToolTip.SetToolTip(NameBox, "ID: " + featured.features.featured_author.id);
            NameBox.LinkClicked += delegate {Process.Start(featured.features.featured_author.url); };

            // Featured File //
            FileBox.ImageLocation = featured.features.featured_file.thumbnail;
            FileNameBox.Text = "Made by " + featured.features.featured_file.user + " - click to view";
            ToolTip FileBoxToolTip = new ToolTip();
            FileBoxToolTip.SetToolTip(FileNameBox, "ID: " + featured.features.featured_file.id);
            FileNameBox.LinkClicked += delegate { Process.Start(featured.features.featured_file.url); };
            FileCost.Text = "$" + featured.features.featured_file.cost;
            FileName.Text = featured.features.featured_file.item;
            FileRating.Text = "Rating: " + featured.features.featured_file.rating.Substring(0,1) + "/5" ;
            FileSales.Text = "Sales: " + featured.features.featured_file.sales;
            FileCategory.Text = "Category: " + featured.features.featured_file.category;

            // "Free File" //
        }
    }
}

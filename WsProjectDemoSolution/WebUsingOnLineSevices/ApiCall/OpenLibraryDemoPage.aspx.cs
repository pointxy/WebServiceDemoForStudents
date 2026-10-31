using Newtonsoft.Json;
//In Package Manager Console: Install-Package Newtonsoft.Json
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http; // add reference
using System.Threading.Tasks;
using System.Web.UI;


namespace WebUsingOnLineSevices.ApiCall
{
    public partial class OpenLibraryDemoPage : System.Web.UI.Page
    {

        // Reuse HttpClient (best practice)
        private static readonly HttpClient http = new HttpClient
        {
            BaseAddress = new Uri("https://openlibrary.org/")
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtQuery.Text = "harry potter";
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // WebForms event handlers are not async by default.
            // Use RegisterAsyncTask to run async code safely.
            RegisterAsyncTask(new PageAsyncTask(RunSearchAsync));
        }

        private async Task RunSearchAsync()
        {
            lblStatus.CssClass = "";
            lblStatus.Text = "";

            string q = (txtQuery.Text ?? "").Trim();
            if (q.Length < 2)
            {
                lblStatus.CssClass = "error";
                lblStatus.Text = "Please type at least 2 characters.";
                gvResults.DataSource = null;
                gvResults.DataBind();
                return;
            }

            try
            {
                // Search API: /search.json?q=...
                // Note: Open Library has tuned /search.json defaults; you can request fields if needed. :contentReference[oaicite:1]{index=1}
                string url = "search.json?q=" + Uri.EscapeDataString(q) + "&limit=20";

                using (var resp = await http.GetAsync(url))
                {
                    resp.EnsureSuccessStatusCode();
                    string json = await resp.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<OpenLibrarySearchResponse>(json);

                    var rows = (data?.docs ?? new List<OpenLibraryDoc>())
                        .Select(d => new BookRow
                        {
                            Title = d.title ?? "",
                            Author = (d.author_name != null && d.author_name.Count > 0)
                                ? string.Join(", ", d.author_name.Take(3))
                                : "",
                            FirstPublishYear = d.first_publish_year?.ToString() ?? "",
                            OpenLibraryUrl = !string.IsNullOrWhiteSpace(d.key)
                                ? "https://openlibrary.org" + d.key
                                : ""
                        })
                        .ToList();

                    gvResults.DataSource = rows;
                    gvResults.DataBind();

                    lblStatus.Text = $"Results: {rows.Count}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.CssClass = "error";
                lblStatus.Text = "Error calling Open Library: " + Server.HtmlEncode(ex.Message);
                gvResults.DataSource = null;
                gvResults.DataBind();
            }
        }

        // --- DTOs for JSON ---

        public class OpenLibrarySearchResponse
        {
            public int numFound { get; set; }
            public List<OpenLibraryDoc> docs { get; set; }
        }

        public class OpenLibraryDoc
        {
            public string key { get; set; }                 // e.g. "/works/OL82563W"
            public string title { get; set; }
            public List<string> author_name { get; set; }
            public int? first_publish_year { get; set; }
        }

        // --- Grid row model ---
        public class BookRow
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string FirstPublishYear { get; set; }
            public string OpenLibraryUrl { get; set; }
        }
    }
}



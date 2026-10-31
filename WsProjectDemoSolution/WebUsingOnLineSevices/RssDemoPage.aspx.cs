using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.ServiceModel.Syndication;
//Click Add Reference…
//Go to Assemblies → Framework
// Check System.ServiceModel

namespace WebUsingOnLineSevices
{
    public partial class RssDemoPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            gvItems.DataSource = null;
            gvItems.DataBind();

            try
            {
                string url = (txtUrl.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(url))
                {
                    lblStatus.Text = "Please enter an RSS URL.";
                    return;
                }

                using (XmlReader reader = XmlReader.Create(url))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    if (feed == null)
                    {
                        lblStatus.Text = "Could not load RSS feed.";
                        return;
                    }

                    var items = feed.Items
                        .Take(15)
                        .Select(i => new RssItem
                        {
                            Title = i.Title?.Text ?? "",
                            Published = i.PublishDate != DateTimeOffset.MinValue
                                ? i.PublishDate.LocalDateTime.ToString("yyyy-MM-dd HH:mm")
                                : "",
                            Link = i.Links.FirstOrDefault()?.Uri?.ToString() ?? ""
                        })
                        .ToList();

                    gvItems.DataSource = items;
                    gvItems.DataBind();

                    lblStatus.Text = $"Loaded {items.Count} items from: {Server.HtmlEncode(feed.Title?.Text ?? "RSS")}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + Server.HtmlEncode(ex.Message);
            }
        }

        public class RssItem
        {
            public string Title { get; set; }
            public string Published { get; set; }
            public string Link { get; set; }
        }
    }

}

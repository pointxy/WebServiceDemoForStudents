using System;
using System.Web.Services.Protocols;
using System.Web.UI.WebControls;
using WebUsingOnLineSevices.MyDbServiceRef;

namespace WebUsingOnLineSevices.MyDbService
{
    // Button Click -> code behind -> SOAP proxy (MyDbServiceRef.WebService1) -> ASMX -> BLL -> DAL -> Access
    public partial class ArticlesPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserDropDowns();
                LoadAllArticles();
            }
        }

        // Fill the 3 author DropDownLists from the service (Text = user name, Value = user ID)
        private void LoadUserDropDowns()
        {
            try
            {
                WebService1 service = new WebService1();
                MyUser[] users = service.UserList();

                DropDownList[] lists = { ddlFilterUser, ddlAddAuthor, ddlEditAuthor };
                foreach (DropDownList ddl in lists)
                {
                    ddl.Items.Clear();
                    foreach (MyUser u in users)
                    {
                        ddl.Items.Add(new ListItem(u.FirstName + " " + u.LastName, u.UserID.ToString()));
                    }
                }
            }
            catch (SoapException ex)
            {
                lblArticleList.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblArticleList.Text = "Could not load users: cannot connect to the Web Service.";
            }
        }

        // ===== GetAllArticles =====
        protected void btnGetAllArticles_Click(object sender, EventArgs e)
        {
            LoadAllArticles();
        }

        private void LoadAllArticles()
        {
            try
            {
                WebService1 service = new WebService1();
                Article[] articles = service.GetAllArticles();

                ShowArticles(articles);
                lblArticleList.Text = articles.Length == 0 ? "No articles found." : "All articles: " + articles.Length;
            }
            catch (SoapException ex)
            {
                lblArticleList.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblArticleList.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== GetArticlesByUser =====
        protected void btnGetArticlesByUser_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = int.Parse(ddlFilterUser.SelectedValue);

                WebService1 service = new WebService1();
                Article[] articles = service.GetArticlesByUser(userId);

                ShowArticles(articles);
                lblArticleList.Text = articles.Length == 0
                    ? "No articles found for " + Server.HtmlEncode(ddlFilterUser.SelectedItem.Text) + "."
                    : "Articles of " + Server.HtmlEncode(ddlFilterUser.SelectedItem.Text) + ": " + articles.Length;
            }
            catch (SoapException ex)
            {
                lblArticleList.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblArticleList.Text = "Could not connect to the Web Service.";
            }
        }

        // The same array is shown in 3 different controls (no extra service call)
        private void ShowArticles(Article[] articles)
        {
            // A. ListBox - add the items one by one (Text = caption, Value = ID)
            lstArticles.Items.Clear();
            foreach (Article a in articles)
            {
                lstArticles.Items.Add(new ListItem(a.Caption, a.ArticleId.ToString()));
            }

            // B. Repeater - custom HTML from the ItemTemplate
            rptArticles.DataSource = articles;
            rptArticles.DataBind();

            // C. GridView - a table with explicit columns
            gvArticles.DataSource = articles;
            gvArticles.DataBind();
        }

        // ===== GetArticleById =====
        protected void btnGetArticleById_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtGetArticleId.Text, out id))
            {
                lblGetArticleById.Text = "Please enter a number.";
                return;
            }

            try
            {
                WebService1 service = new WebService1();
                Article article = service.GetArticleById(id);

                if (article == null)
                    lblGetArticleById.Text = "Article not found (null).";
                else
                    lblGetArticleById.Text = Server.HtmlEncode("ID: " + article.ArticleId + ", " + article.Caption
                        + ", by " + article.Author.FirstName + " " + article.Author.LastName);
            }
            catch (SoapException ex)
            {
                // ArticleBLL threw an Exception (for example: id = 0)
                lblGetArticleById.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblGetArticleById.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== AddArticle =====
        protected void btnAddArticle_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Read the form and build the proxy object
                Article article = new Article();
                article.Caption = txtAddCaption.Text;
                article.ArticleText = txtAddText.Text;
                article.Publish = chkAddPublish.Checked;
                article.CreateDate = DateTime.Now;
                article.Author = new MyUser();
                article.Author.UserID = int.Parse(ddlAddAuthor.SelectedValue);

                // 2. Call the WebMethod
                WebService1 service = new WebService1();
                int result = service.AddArticle(article);

                // 3. Show the result
                lblAddArticle.Text = AddResultText(result);
                LoadAllArticles();
            }
            catch (SoapException ex)
            {
                // ArticleBLL threw an Exception (for example: empty caption)
                lblAddArticle.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblAddArticle.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UpdateArticle =====
        protected void btnLoadSelectedArticle_Click(object sender, EventArgs e)
        {
            if (lstArticles.SelectedItem == null)
            {
                lblUpdateArticle.Text = "Please select an article in the ListBox.";
                return;
            }

            try
            {
                // The ListBox Value holds the article ID
                int id = int.Parse(lstArticles.SelectedValue);

                WebService1 service = new WebService1();
                Article article = service.GetArticleById(id);

                if (article == null)
                {
                    lblUpdateArticle.Text = "Article not found.";
                    return;
                }

                txtEditArticleId.Text = article.ArticleId.ToString();
                txtEditCaption.Text = article.Caption;
                txtEditText.Text = article.ArticleText;
                ddlEditAuthor.SelectedValue = article.Author.UserID.ToString();
                chkEditPublish.Checked = article.Publish;
                lblUpdateArticle.Text = "Article loaded. Change the fields and click Update Article.";
            }
            catch (SoapException ex)
            {
                lblUpdateArticle.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUpdateArticle.Text = "Could not connect to the Web Service.";
            }
        }

        protected void btnUpdateArticle_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtEditArticleId.Text, out id))
            {
                lblUpdateArticle.Text = "Please load an article first.";
                return;
            }

            try
            {
                Article article = new Article();
                article.ArticleId = id;
                article.Caption = txtEditCaption.Text;
                article.ArticleText = txtEditText.Text;
                article.Publish = chkEditPublish.Checked;
                article.Author = new MyUser();
                article.Author.UserID = int.Parse(ddlEditAuthor.SelectedValue);

                WebService1 service = new WebService1();
                int result = service.UpdateArticle(article);

                lblUpdateArticle.Text = RowsResultText(result);
                LoadAllArticles();
            }
            catch (SoapException ex)
            {
                lblUpdateArticle.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUpdateArticle.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== DeleteArticle =====
        protected void btnDeleteArticle_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtDeleteArticleId.Text, out id))
            {
                lblDeleteArticle.Text = "Please enter a number.";
                return;
            }

            try
            {
                WebService1 service = new WebService1();
                int result = service.DeleteArticle(id);

                lblDeleteArticle.Text = RowsResultText(result);
                LoadAllArticles();
            }
            catch (SoapException ex)
            {
                // ArticleBLL threw an Exception (for example: the article does not exist)
                lblDeleteArticle.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblDeleteArticle.Text = "Could not connect to the Web Service.";
            }
        }

        // =========================
        // Result texts
        // =========================
        // Add: positive = new ID, -1 = failed
        private string AddResultText(int result)
        {
            if (result > 0)
                return "Added. New ID = " + result;
            return "Operation failed (" + result + ").";
        }

        // Update / Delete: positive = rows affected, 0 = nothing changed, -1 = failed
        private string RowsResultText(int result)
        {
            if (result > 0)
                return result + " row(s) affected.";
            if (result == 0)
                return "No rows were affected (0).";
            return "Operation failed (-1).";
        }

        // Shows only the short message sent by the service (no stack trace)
        private string SoapErrorText(SoapException ex)
        {
            string message = ex.Message.Split('\n')[0];
            int pos = message.IndexOf("SoapException:");
            if (pos >= 0)
                message = message.Substring(pos + "SoapException:".Length);

            return Server.HtmlEncode("Service error: " + message.Trim());
        }
    }
}

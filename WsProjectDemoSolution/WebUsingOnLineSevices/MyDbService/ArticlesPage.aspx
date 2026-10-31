<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticlesPage.aspx.cs" Inherits="WebUsingOnLineSevices.MyDbService.ArticlesPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Articles - MyDbWebService Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .box { padding: 10px 16px; border: 1px solid #ddd; border-radius: 8px; max-width: 900px; margin-bottom: 14px; }
        .row { margin: 8px 0; }
        .label { display: inline-block; width: 120px; vertical-align: top; }
        .result { font-weight: bold; color: #0645ad; }
        .card { border: 1px solid #ccc; border-radius: 6px; padding: 6px 10px; margin: 4px 0; background: #f8f8ff; }
        .hint { color: #666; font-size: 0.9em; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <p><a href="../Default.aspx">&larr; Home</a></p>
        <h1>Articles</h1>
        <p class="hint">Every button calls one WebMethod of WebService1.asmx through the SOAP proxy (Web Reference: MyDbServiceRef).
            For invalid input, ArticleBLL throws an Exception - the client catches it as a SoapException.</p>

        <!-- ===== GetAllArticles / GetArticlesByUser ===== -->
        <div class="box">
            <h2>GetAllArticles</h2>
            <asp:Button ID="btnGetAllArticles" runat="server" Text="Get All Articles" OnClick="btnGetAllArticles_Click" />

            <h2>GetArticlesByUser</h2>
            <div class="row">
                <span class="label">Author:</span>
                <asp:DropDownList ID="ddlFilterUser" runat="server" />
                <asp:Button ID="btnGetArticlesByUser" runat="server" Text="Get Articles By User" OnClick="btnGetArticlesByUser_Click" />
            </div>
            <asp:Label ID="lblArticleList" runat="server" CssClass="result" />

            <h3>A. ListBox</h3>
            <asp:ListBox ID="lstArticles" runat="server" Rows="6" Width="400" />
            <span class="hint">Text = caption, Value = article ID</span>

            <h3>B. Repeater</h3>
            <asp:Repeater ID="rptArticles" runat="server">
                <ItemTemplate>
                    <div class="card">
                        <b><%#: Eval("Caption") %></b> (#<%#: Eval("ArticleId") %>)
                        by <%#: Eval("Author.FirstName") %> <%#: Eval("Author.LastName") %>,
                        <%#: Eval("CreateDate", "{0:yyyy-MM-dd}") %>, Published: <%#: Eval("Publish") %><br />
                        <%#: Eval("ArticleText") %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <h3>C. GridView</h3>
            <asp:GridView ID="gvArticles" runat="server" AutoGenerateColumns="false" CellPadding="6">
                <Columns>
                    <asp:BoundField DataField="ArticleId" HeaderText="ID" />
                    <asp:BoundField DataField="Caption" HeaderText="Caption" />
                    <asp:TemplateField HeaderText="Author">
                        <ItemTemplate><%#: Eval("Author.FirstName") %> <%#: Eval("Author.LastName") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreateDate" HeaderText="Created" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:CheckBoxField DataField="Publish" HeaderText="Published" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- ===== GetArticleById ===== -->
        <div class="box">
            <h2>GetArticleById</h2>
            <div class="row">
                <span class="label">Article ID:</span>
                <asp:TextBox ID="txtGetArticleId" runat="server" Width="80" />
                <asp:Button ID="btnGetArticleById" runat="server" Text="Get Article" OnClick="btnGetArticleById_Click" />
            </div>
            <div class="hint">Try 0 to see the exception from ArticleBLL.</div>
            <asp:Label ID="lblGetArticleById" runat="server" CssClass="result" />
        </div>

        <!-- ===== AddArticle ===== -->
        <div class="box">
            <h2>AddArticle</h2>
            <div class="row"><span class="label">Caption:</span><asp:TextBox ID="txtAddCaption" runat="server" Width="300" /></div>
            <div class="row"><span class="label">Text:</span><asp:TextBox ID="txtAddText" runat="server" TextMode="MultiLine" Rows="3" Width="300" /></div>
            <div class="row"><span class="label">Author:</span><asp:DropDownList ID="ddlAddAuthor" runat="server" /></div>
            <div class="row"><span class="label">Published:</span><asp:CheckBox ID="chkAddPublish" runat="server" /></div>
            <asp:Button ID="btnAddArticle" runat="server" Text="Add Article" OnClick="btnAddArticle_Click" />
            <div class="hint">Leave the caption empty to see the exception from ArticleBLL.</div>
            <asp:Label ID="lblAddArticle" runat="server" CssClass="result" />
        </div>

        <!-- ===== UpdateArticle ===== -->
        <div class="box">
            <h2>UpdateArticle</h2>
            <div class="row">
                <span class="hint">1. Select an article in the ListBox above, 2. load it (GetArticleById), 3. change fields, 4. update.</span>
            </div>
            <asp:Button ID="btnLoadSelectedArticle" runat="server" Text="Load Selected Article" OnClick="btnLoadSelectedArticle_Click" />
            <div class="row"><span class="label">Article ID:</span><asp:TextBox ID="txtEditArticleId" runat="server" Width="80" ReadOnly="true" /></div>
            <div class="row"><span class="label">Caption:</span><asp:TextBox ID="txtEditCaption" runat="server" Width="300" /></div>
            <div class="row"><span class="label">Text:</span><asp:TextBox ID="txtEditText" runat="server" TextMode="MultiLine" Rows="3" Width="300" /></div>
            <div class="row"><span class="label">Author:</span><asp:DropDownList ID="ddlEditAuthor" runat="server" /></div>
            <div class="row"><span class="label">Published:</span><asp:CheckBox ID="chkEditPublish" runat="server" /></div>
            <asp:Button ID="btnUpdateArticle" runat="server" Text="Update Article" OnClick="btnUpdateArticle_Click" />
            <asp:Label ID="lblUpdateArticle" runat="server" CssClass="result" />
        </div>

        <!-- ===== DeleteArticle ===== -->
        <div class="box">
            <h2>DeleteArticle</h2>
            <div class="row">
                <span class="label">Article ID:</span>
                <asp:TextBox ID="txtDeleteArticleId" runat="server" Width="80" />
                <asp:Button ID="btnDeleteArticle" runat="server" Text="Delete Article" OnClick="btnDeleteArticle_Click" />
            </div>
            <div class="hint">Try a missing ID (e.g. 9999) to see the exception "המאמר לא קיים".</div>
            <asp:Label ID="lblDeleteArticle" runat="server" CssClass="result" />
        </div>
    </form>
</body>
</html>

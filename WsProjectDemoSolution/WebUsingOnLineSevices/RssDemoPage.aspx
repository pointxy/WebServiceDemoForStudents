<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RssDemoPage.aspx.cs" Inherits="WebUsingOnLineSevices.RssDemoPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RSS Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .box { border: 1px solid #ccc; padding: 15px; border-radius: 10px; max-width: 900px; }
        .row { margin: 8px 0; }
        .label { display:inline-block; width: 90px; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div class="box">
        <h2>RSS Feed Demo (XML)</h2>

        <div class="row">
            <span class="label">RSS URL:</span>
            <asp:TextBox ID="txtUrl" runat="server" Width="650"
                Text="https://feeds.bbci.co.uk/news/world/rss.xml" />
        </div>

        <div class="row">
            <asp:Button ID="btnLoad" runat="server" Text="Load Feed" OnClick="btnLoad_Click" />
        </div>

        <div class="row">
            <asp:Label ID="lblStatus" runat="server" />
        </div>

        <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="false"
            BorderWidth="1" CellPadding="6" GridLines="Both">
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Published" HeaderText="Published" />
                <asp:HyperLinkField DataTextField="Link" DataNavigateUrlFields="Link"
                    HeaderText="Link" Text="Open" Target="_blank" />
            </Columns>
        </asp:GridView>
    </div>
</form>
</body>
</html>

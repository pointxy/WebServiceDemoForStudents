<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="OpenLibraryDemoPage.aspx.cs" Inherits="WebUsingOnLineSevices.ApiCall.OpenLibraryDemoPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Open Library API Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .row { margin-bottom: 10px; }
        .hint { color: #555; font-size: 0.9em; }
        .error { color: #b00020; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <h2>Open Library Search API Demo</h2>

    <div class="row">
        <asp:Label runat="server" AssociatedControlID="txtQuery" Text="Search:" />
        <asp:TextBox runat="server" ID="txtQuery" Width="360" />
        <asp:Button runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click" />
    </div>

    <div class="row hint">
        Uses: https://openlibrary.org/search.json?q=YOUR_QUERY
    </div>

    <div class="row">
        <asp:Label runat="server" ID="lblStatus" />
    </div>

    <asp:GridView runat="server" ID="gvResults" AutoGenerateColumns="False" EmptyDataText="No results."
        GridLines="Horizontal" CellPadding="6">
        <Columns>
            <asp:BoundField DataField="Title" HeaderText="Title" />
            <asp:BoundField DataField="Author" HeaderText="Author" />
            <asp:BoundField DataField="FirstPublishYear" HeaderText="First Publish Year" />
            <asp:HyperLinkField DataTextField="OpenLibraryUrl" HeaderText="Open Library"
                DataNavigateUrlFields="OpenLibraryUrl" Target="_blank" />
        </Columns>
    </asp:GridView>
</form>
</body>
</html>

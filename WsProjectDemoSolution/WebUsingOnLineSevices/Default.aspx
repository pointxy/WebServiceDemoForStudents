<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebUsingOnLineSevices._Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web Services Demo - Home</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .box { padding: 10px 20px; border: 1px solid #ddd; border-radius: 8px; max-width: 600px; margin-bottom: 16px; }
        li { margin: 6px 0; }
        .small { color: #666; font-size: 0.9em; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Web Services Demo</h1>

        <div class="box">
            <h2>External Web Services</h2>
            <ul>
                <li><a href="CalculatorServicePage.aspx">Calculator Service</a> <span class="small">(SOAP)</span></li>
                <li><a href="CountryInfoServicePage.aspx">Country Information Service</a> <span class="small">(SOAP)</span></li>
                <li><a href="RssDemoPage.aspx">RSS Demo</a> <span class="small">(RSS / XML)</span></li>
                <li><a href="ApiCall/OpenLibraryDemoPage.aspx">Open Library REST Demo</a> <span class="small">(REST / JSON)</span></li>
            </ul>
        </div>

        <div class="box">
            <h2>Our Database Web Service</h2>
            <p class="small">MyDbWebService (WebService1.asmx) must be running.</p>
            <ul>
                <li><a href="MyDbService/UsersPage.aspx">Users</a></li>
                <li><a href="MyDbService/RolesPage.aspx">Roles</a></li>
                <li><a href="MyDbService/ArticlesPage.aspx">Articles</a></li>
            </ul>
        </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CountryInfoServicePage.aspx.cs" Inherits="WebUsingOnLineSevices.CountryInfoServicePage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CountryInfoService Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .row { margin: 10px 0; }
        .label { display:inline-block; width: 140px; }
        .box { padding: 10px; border: 1px solid #ddd; border-radius: 8px; max-width: 900px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="box">
            <h2>CountryInfoService Demo (SOAP/XML)</h2>

            <div class="row">
                <span class="label">Search contains:</span>
                <asp:TextBox ID="txtFilter" runat="server" Width="240" Text="is" />
                <asp:Button ID="btnLoad" runat="server" Text="Load Countries" OnClick="btnLoad_Click" />
            </div>

            <div class="row">
                <asp:Label ID="lblStatus" runat="server" />
            </div>

            <asp:GridView ID="gvCountries" runat="server" AutoGenerateColumns="false" BorderWidth="1"
                CellPadding="6" GridLines="Both">
                <Columns>
                    <asp:BoundField DataField="sISOCode" HeaderText="ISO Code" />
                    <asp:BoundField DataField="sName" HeaderText="Country Name" />
                </Columns>
            </asp:GridView>

            <hr />

            <h3>Details by ISO Code</h3>
            <div class="row">
                <span class="label">ISO Code (e.g. IL):</span>
                <asp:TextBox ID="txtIso" runat="server" Width="80" Text="IL" />
                <asp:Button ID="btnDetails" runat="server" Text="Get Details" OnClick="btnDetails_Click" />
            </div>

            <div class="row">
                <asp:Label ID="lblDetails" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>

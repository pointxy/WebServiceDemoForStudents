<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalculatorServicePage.aspx.cs" Inherits="WebUsingOnLineSevices.CalculatorServicePage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calculator SOAP Web Service Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .box { border: 1px solid #ccc; padding: 20px; border-radius: 10px; width: 420px; }
        .row { margin: 10px 0; }
        .label { display: inline-block; width: 80px; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div class="box">
        <h2>Calculator Web Service</h2>

        <div class="row">
            <span class="label">Number A:</span>
            <asp:TextBox ID="txtA" runat="server" />
        </div>

        <div class="row">
            <span class="label">Number B:</span>
            <asp:TextBox ID="txtB" runat="server" />
        </div>

        <div class="row">
            <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />
            <asp:Button ID="btnSub" runat="server" Text="Subtract" OnClick="btnSub_Click" />
            <asp:Button ID="btnMul" runat="server" Text="Multiply" OnClick="btnMul_Click" />
            <asp:Button ID="btnDiv" runat="server" Text="Divide" OnClick="btnDiv_Click" />
        </div>
        
        <div class="row">
            <asp:Label ID="lblResult" runat="server" Font-Bold="true" Text=""></asp:Label>
         
        </div>
    </div>
</form>
</body>
</html>
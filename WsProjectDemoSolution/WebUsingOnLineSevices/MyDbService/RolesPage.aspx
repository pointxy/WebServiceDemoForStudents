<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RolesPage.aspx.cs" Inherits="WebUsingOnLineSevices.MyDbService.RolesPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Roles - MyDbWebService Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .box { padding: 10px 16px; border: 1px solid #ddd; border-radius: 8px; max-width: 900px; margin-bottom: 14px; }
        .row { margin: 8px 0; }
        .label { display: inline-block; width: 120px; }
        .result { font-weight: bold; color: #0645ad; }
        .card { display: inline-block; border: 1px solid #ccc; border-radius: 6px; padding: 6px 10px; margin: 4px; background: #f8f8ff; }
        .hint { color: #666; font-size: 0.9em; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <p><a href="../Default.aspx">&larr; Home</a></p>
        <h1>Roles</h1>
        <p class="hint">Every button calls one WebMethod of WebService1.asmx through the SOAP proxy (Web Reference: MyDbServiceRef).</p>

        <!-- ===== GetAllRoles ===== -->
        <div class="box">
            <h2>GetAllRoles</h2>
            <asp:Button ID="btnGetAllRoles" runat="server" Text="Get All Roles" OnClick="btnGetAllRoles_Click" />
            <asp:Label ID="lblGetAllRoles" runat="server" CssClass="result" />

            <h3>A. ListBox</h3>
            <asp:ListBox ID="lstRoles" runat="server" Rows="6" Width="300" />
            <span class="hint">Text = role name, Value = role ID</span>

            <h3>B. Repeater</h3>
            <asp:Repeater ID="rptRoles" runat="server">
                <ItemTemplate>
                    <div class="card">#<%#: Eval("RoleId") %> &ndash; <b><%#: Eval("RoleName") %></b></div>
                </ItemTemplate>
            </asp:Repeater>

            <h3>C. GridView</h3>
            <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="false" CellPadding="6">
                <Columns>
                    <asp:BoundField DataField="RoleId" HeaderText="ID" />
                    <asp:BoundField DataField="RoleName" HeaderText="Role Name" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- ===== GetRoleById ===== -->
        <div class="box">
            <h2>GetRoleById</h2>
            <div class="row">
                <span class="label">Role ID:</span>
                <asp:TextBox ID="txtGetRoleId" runat="server" Width="80" />
                <asp:Button ID="btnGetRoleById" runat="server" Text="Get Role" OnClick="btnGetRoleById_Click" />
            </div>
            <asp:Label ID="lblGetRoleById" runat="server" CssClass="result" />
        </div>

        <!-- ===== AddRole ===== -->
        <div class="box">
            <h2>AddRole</h2>
            <div class="row">
                <span class="label">Role name:</span>
                <asp:TextBox ID="txtAddRoleName" runat="server" Width="200" />
                <asp:Button ID="btnAddRole" runat="server" Text="Add Role" OnClick="btnAddRole_Click" />
            </div>
            <asp:Label ID="lblAddRole" runat="server" CssClass="result" />
        </div>

        <!-- ===== UpdateRole ===== -->
        <div class="box">
            <h2>UpdateRole</h2>
            <div class="row">
                <span class="hint">1. Select a role in the ListBox above, 2. load it, 3. change the name, 4. update.</span>
            </div>
            <div class="row">
                <asp:Button ID="btnLoadSelectedRole" runat="server" Text="Load Selected Role" OnClick="btnLoadSelectedRole_Click" />
            </div>
            <div class="row">
                <span class="label">Role ID:</span>
                <asp:TextBox ID="txtEditRoleId" runat="server" Width="80" ReadOnly="true" />
            </div>
            <div class="row">
                <span class="label">Role name:</span>
                <asp:TextBox ID="txtEditRoleName" runat="server" Width="200" />
                <asp:Button ID="btnUpdateRole" runat="server" Text="Update Role" OnClick="btnUpdateRole_Click" />
            </div>
            <asp:Label ID="lblUpdateRole" runat="server" CssClass="result" />
        </div>

        <!-- ===== DeleteRole ===== -->
        <div class="box">
            <h2>DeleteRole</h2>
            <div class="row">
                <span class="label">Role ID:</span>
                <asp:TextBox ID="txtDeleteRoleId" runat="server" Width="80" />
                <asp:Button ID="btnDeleteRole" runat="server" Text="Delete Role" OnClick="btnDeleteRole_Click" />
            </div>
            <div class="hint">Business rule (in the BLL): a role assigned to users cannot be deleted. Try role 1.</div>
            <asp:Label ID="lblDeleteRole" runat="server" CssClass="result" />
        </div>
    </form>
</body>
</html>

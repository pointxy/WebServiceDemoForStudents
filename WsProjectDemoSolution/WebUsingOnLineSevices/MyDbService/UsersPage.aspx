<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UsersPage.aspx.cs" Inherits="WebUsingOnLineSevices.MyDbService.UsersPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Users - MyDbWebService Demo</title>
    <style>
        body { font-family: Arial; margin: 20px; }
        .box { padding: 10px 16px; border: 1px solid #ddd; border-radius: 8px; max-width: 900px; margin-bottom: 14px; }
        .row { margin: 8px 0; }
        .label { display: inline-block; width: 120px; }
        .result { font-weight: bold; color: #0645ad; }
        .card { border: 1px solid #ccc; border-radius: 6px; padding: 6px 10px; margin: 4px 0; background: #f8f8ff; }
        .hint { color: #666; font-size: 0.9em; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <p><a href="../Default.aspx">&larr; Home</a></p>
        <h1>Users</h1>
        <p class="hint">Every button calls one WebMethod of WebService1.asmx through the SOAP proxy (Web Reference: MyDbServiceRef).
            The existing WebMethod names are used (UserList, UserGetById, UserAdd, ...). Passwords are never shown.</p>

        <!-- ===== UserList (GetAllUsers) ===== -->
        <div class="box">
            <h2>UserList <span class="hint">(get all users)</span></h2>
            <asp:Button ID="btnUserList" runat="server" Text="Get All Users" OnClick="btnUserList_Click" />
            <asp:Label ID="lblUserList" runat="server" CssClass="result" />

            <h3>A. ListBox</h3>
            <asp:ListBox ID="lstUsers" runat="server" Rows="6" Width="400" />
            <span class="hint">Text = name and email, Value = user ID</span>

            <h3>B. Repeater</h3>
            <asp:Repeater ID="rptUsers" runat="server">
                <ItemTemplate>
                    <div class="card">
                        <b><%#: Eval("FirstName") %> <%#: Eval("LastName") %></b> (#<%#: Eval("UserID") %>)<br />
                        Email: <%#: Eval("UserEmail") %> | Role: <%#: Eval("UserRole.RoleName") %> | Active: <%#: Eval("IsActive") %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <h3>C. GridView</h3>
            <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CellPadding="6">
                <Columns>
                    <asp:BoundField DataField="UserID" HeaderText="ID" />
                    <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                    <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                    <asp:BoundField DataField="UserEmail" HeaderText="Email" />
                    <asp:BoundField DataField="DOB" HeaderText="Date of Birth" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:TemplateField HeaderText="Role">
                        <ItemTemplate><%#: Eval("UserRole.RoleName") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
                </Columns>
            </asp:GridView>
        </div>

        <!-- ===== UserGetById ===== -->
        <div class="box">
            <h2>UserGetById</h2>
            <div class="row">
                <span class="label">User ID:</span>
                <asp:TextBox ID="txtGetUserId" runat="server" Width="80" />
                <asp:Button ID="btnUserGetById" runat="server" Text="Get User" OnClick="btnUserGetById_Click" />
            </div>
            <asp:Label ID="lblUserGetById" runat="server" CssClass="result" />
        </div>

        <!-- ===== UserAdd ===== -->
        <div class="box">
            <h2>UserAdd</h2>
            <div class="row"><span class="label">First name:</span><asp:TextBox ID="txtAddFirstName" runat="server" /></div>
            <div class="row"><span class="label">Last name:</span><asp:TextBox ID="txtAddLastName" runat="server" /></div>
            <div class="row"><span class="label">Email:</span><asp:TextBox ID="txtAddEmail" runat="server" Width="220" /></div>
            <div class="row"><span class="label">Password:</span><asp:TextBox ID="txtAddPassword" runat="server" TextMode="Password" /></div>
            <div class="row"><span class="label">Date of birth:</span><asp:TextBox ID="txtAddDob" runat="server" TextMode="Date" /> <span class="hint">(optional)</span></div>
            <div class="row"><span class="label">Role:</span><asp:DropDownList ID="ddlAddRole" runat="server" /></div>
            <div class="row"><span class="label">Active:</span><asp:CheckBox ID="chkAddActive" runat="server" Checked="true" /></div>
            <asp:Button ID="btnUserAdd" runat="server" Text="Add User" OnClick="btnUserAdd_Click" />
            <asp:Label ID="lblUserAdd" runat="server" CssClass="result" />
        </div>

        <!-- ===== UserUpdate ===== -->
        <div class="box">
            <h2>UserUpdate</h2>
            <div class="row">
                <span class="hint">1. Select a user in the ListBox above, 2. load it (UserGetById), 3. change fields, 4. update.</span>
            </div>
            <asp:Button ID="btnLoadSelectedUser" runat="server" Text="Load Selected User" OnClick="btnLoadSelectedUser_Click" />
            <div class="row"><span class="label">User ID:</span><asp:TextBox ID="txtEditUserId" runat="server" Width="80" ReadOnly="true" /></div>
            <div class="row"><span class="label">First name:</span><asp:TextBox ID="txtEditFirstName" runat="server" /></div>
            <div class="row"><span class="label">Last name:</span><asp:TextBox ID="txtEditLastName" runat="server" /></div>
            <div class="row"><span class="label">Email:</span><asp:TextBox ID="txtEditEmail" runat="server" Width="220" /></div>
            <div class="row"><span class="label">New password:</span><asp:TextBox ID="txtEditPassword" runat="server" TextMode="Password" /> <span class="hint">(leave empty to keep the current password)</span></div>
            <div class="row"><span class="label">Date of birth:</span><asp:TextBox ID="txtEditDob" runat="server" TextMode="Date" /></div>
            <div class="row"><span class="label">Role:</span><asp:DropDownList ID="ddlEditRole" runat="server" /></div>
            <div class="row"><span class="label">Active:</span><asp:CheckBox ID="chkEditActive" runat="server" /></div>
            <asp:Button ID="btnUserUpdate" runat="server" Text="Update User" OnClick="btnUserUpdate_Click" />
            <asp:Label ID="lblUserUpdate" runat="server" CssClass="result" />
        </div>

        <!-- ===== UserDelete ===== -->
        <div class="box">
            <h2>UserDelete</h2>
            <div class="row">
                <span class="label">User ID:</span>
                <asp:TextBox ID="txtDeleteUserId" runat="server" Width="80" />
                <asp:Button ID="btnUserDelete" runat="server" Text="Delete User" OnClick="btnUserDelete_Click" />
            </div>
            <div class="hint">Business rule (in the BLL): a user who wrote articles cannot be deleted. Try user 1.</div>
            <asp:Label ID="lblUserDelete" runat="server" CssClass="result" />
        </div>

        <!-- ===== ValidateUser ===== -->
        <div class="box">
            <h2>ValidateUser <span class="hint">(checks the data only - this is not a login)</span></h2>
            <div class="row"><span class="label">First name:</span><asp:TextBox ID="txtValidateFirstName" runat="server" /></div>
            <div class="row"><span class="label">Last name:</span><asp:TextBox ID="txtValidateLastName" runat="server" /></div>
            <div class="row"><span class="label">Email:</span><asp:TextBox ID="txtValidateEmail" runat="server" Width="220" /></div>
            <div class="row"><span class="label">Role:</span><asp:DropDownList ID="ddlValidateRole" runat="server" /></div>
            <asp:Button ID="btnValidateUser" runat="server" Text="Validate User" OnClick="btnValidateUser_Click" />
            <asp:Label ID="lblValidateUser" runat="server" CssClass="result" />
        </div>

        <!-- ===== UserByEmailPass ===== -->
        <div class="box">
            <h2>UserByEmailPass <span class="hint">(login)</span></h2>
            <div class="row"><span class="label">Email:</span><asp:TextBox ID="txtLoginEmail" runat="server" Width="220" /></div>
            <div class="row"><span class="label">Password:</span><asp:TextBox ID="txtLoginPassword" runat="server" TextMode="Password" /></div>
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
            <asp:Label ID="lblLogin" runat="server" CssClass="result" />
        </div>
    </form>
</body>
</html>

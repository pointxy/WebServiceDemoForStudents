using System;
using System.Web.Services.Protocols;
using System.Web.UI.WebControls;
using WebUsingOnLineSevices.MyDbServiceRef;

namespace WebUsingOnLineSevices.MyDbService
{
    // Button Click -> code behind -> SOAP proxy (MyDbServiceRef.WebService1) -> ASMX -> BLL -> DAL -> Access
    public partial class UsersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRoleDropDowns();
                LoadUsers();
            }
        }

        // Fill the 3 role DropDownLists from the service (Text = role name, Value = role ID)
        private void LoadRoleDropDowns()
        {
            try
            {
                WebService1 service = new WebService1();
                Role[] roles = service.GetAllRoles();

                DropDownList[] lists = { ddlAddRole, ddlEditRole, ddlValidateRole };
                foreach (DropDownList ddl in lists)
                {
                    ddl.DataSource = roles;
                    ddl.DataTextField = "RoleName";
                    ddl.DataValueField = "RoleId";
                    ddl.DataBind();
                }
            }
            catch (SoapException ex)
            {
                lblUserAdd.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserAdd.Text = "Could not load roles: cannot connect to the Web Service.";
            }
        }

        // ===== UserList =====
        protected void btnUserList_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        // One service call - the same list is shown in 3 different controls
        private void LoadUsers()
        {
            try
            {
                WebService1 service = new WebService1();
                MyUser[] users = service.UserList();

                // A. ListBox - add the items one by one (Text = name, Value = ID)
                lstUsers.Items.Clear();
                foreach (MyUser u in users)
                {
                    string text = u.FirstName + " " + u.LastName + " (" + u.UserEmail + ")";
                    lstUsers.Items.Add(new ListItem(text, u.UserID.ToString()));
                }

                // B. Repeater - custom HTML from the ItemTemplate
                rptUsers.DataSource = users;
                rptUsers.DataBind();

                // C. GridView - a table with explicit columns (no password column)
                gvUsers.DataSource = users;
                gvUsers.DataBind();

                lblUserList.Text = users.Length == 0 ? "No users found." : "Loaded " + users.Length + " users.";
            }
            catch (SoapException ex)
            {
                lblUserList.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserList.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UserGetById =====
        protected void btnUserGetById_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtGetUserId.Text, out id))
            {
                lblUserGetById.Text = "Please enter a number.";
                return;
            }

            try
            {
                WebService1 service = new WebService1();
                MyUser user = service.UserGetById(id);

                if (user == null)
                    lblUserGetById.Text = "User not found (null).";
                else
                    lblUserGetById.Text = Server.HtmlEncode("ID: " + user.UserID + ", " + user.FirstName + " " + user.LastName
                        + ", " + user.UserEmail + ", Role: " + user.UserRole.RoleName);
            }
            catch (SoapException ex)
            {
                lblUserGetById.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserGetById.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UserAdd =====
        protected void btnUserAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Read the form and build the proxy object (generated from the WSDL)
                MyUser user = new MyUser();
                user.FirstName = txtAddFirstName.Text;
                user.LastName = txtAddLastName.Text;
                user.UserEmail = txtAddEmail.Text;
                user.UserPass = txtAddPassword.Text;
                user.DOB = ReadDate(txtAddDob.Text);
                user.IsActive = chkAddActive.Checked;
                user.UserRole = new Role();
                user.UserRole.RoleId = int.Parse(ddlAddRole.SelectedValue);

                // 2. Call the WebMethod
                WebService1 service = new WebService1();
                int result = service.UserAdd(user);

                // 3. Show the result
                lblUserAdd.Text = AddResultText(result);
                LoadUsers();
            }
            catch (SoapException ex)
            {
                lblUserAdd.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserAdd.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UserUpdate =====
        protected void btnLoadSelectedUser_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem == null)
            {
                lblUserUpdate.Text = "Please select a user in the ListBox.";
                return;
            }

            try
            {
                // The ListBox Value holds the user ID
                int id = int.Parse(lstUsers.SelectedValue);

                WebService1 service = new WebService1();
                MyUser user = service.UserGetById(id);

                if (user == null)
                {
                    lblUserUpdate.Text = "User not found.";
                    return;
                }

                // Fill the edit form (the service never returns the password)
                txtEditUserId.Text = user.UserID.ToString();
                txtEditFirstName.Text = user.FirstName;
                txtEditLastName.Text = user.LastName;
                txtEditEmail.Text = user.UserEmail;
                txtEditDob.Text = user.DOB.HasValue ? user.DOB.Value.ToString("yyyy-MM-dd") : "";
                ddlEditRole.SelectedValue = user.UserRole.RoleId.ToString();
                chkEditActive.Checked = user.IsActive;
                lblUserUpdate.Text = "User loaded. Change the fields and click Update User.";
            }
            catch (SoapException ex)
            {
                lblUserUpdate.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserUpdate.Text = "Could not connect to the Web Service.";
            }
        }

        protected void btnUserUpdate_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtEditUserId.Text, out id))
            {
                lblUserUpdate.Text = "Please load a user first.";
                return;
            }

            try
            {
                MyUser user = new MyUser();
                user.UserID = id;
                user.FirstName = txtEditFirstName.Text;
                user.LastName = txtEditLastName.Text;
                user.UserEmail = txtEditEmail.Text;
                user.UserPass = txtEditPassword.Text;   // empty = the server keeps the current password
                user.DOB = ReadDate(txtEditDob.Text);
                user.IsActive = chkEditActive.Checked;
                user.UserRole = new Role();
                user.UserRole.RoleId = int.Parse(ddlEditRole.SelectedValue);

                WebService1 service = new WebService1();
                int result = service.UserUpdate(user);

                lblUserUpdate.Text = RowsResultText(result);
                LoadUsers();
            }
            catch (SoapException ex)
            {
                lblUserUpdate.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserUpdate.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UserDelete =====
        protected void btnUserDelete_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtDeleteUserId.Text, out id))
            {
                lblUserDelete.Text = "Please enter a number.";
                return;
            }

            try
            {
                // The existing WebMethod receives a MyUser object - only the ID is needed
                MyUser user = new MyUser();
                user.UserID = id;

                WebService1 service = new WebService1();
                int result = service.UserDelete(user);

                lblUserDelete.Text = RowsResultText(result);
                LoadUsers();
            }
            catch (SoapException ex)
            {
                lblUserDelete.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUserDelete.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== ValidateUser =====
        protected void btnValidateUser_Click(object sender, EventArgs e)
        {
            try
            {
                MyUser user = new MyUser();
                user.FirstName = txtValidateFirstName.Text;
                user.LastName = txtValidateLastName.Text;
                user.UserEmail = txtValidateEmail.Text;
                user.UserRole = new Role();
                user.UserRole.RoleId = int.Parse(ddlValidateRole.SelectedValue);

                WebService1 service = new WebService1();
                bool isValid = service.ValidateUser(user);

                lblValidateUser.Text = isValid ? "true - the data is valid." : "false - the data is not valid.";
            }
            catch (SoapException ex)
            {
                lblValidateUser.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblValidateUser.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UserByEmailPass (login) =====
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                WebService1 service = new WebService1();
                MyUser user = service.UserByEmailPass(txtLoginEmail.Text, txtLoginPassword.Text);

                if (user == null)
                    lblLogin.Text = "Login failed: wrong email or password.";
                else
                    lblLogin.Text = Server.HtmlEncode("Welcome " + user.FirstName + " " + user.LastName
                        + " (Role: " + user.UserRole.RoleName + ")");
            }
            catch (SoapException ex)
            {
                lblLogin.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblLogin.Text = "Could not connect to the Web Service.";
            }
        }

        // =========================
        // Small helpers
        // =========================
        // Empty text = no date (null)
        private DateTime? ReadDate(string text)
        {
            DateTime date;
            if (DateTime.TryParse(text, out date))
                return date;
            return null;
        }

        // Add: positive = new ID, -1 = failed
        private string AddResultText(int result)
        {
            if (result > 0)
                return "Added. New ID = " + result;
            return "Operation failed (" + result + "): invalid data, missing password or email already exists.";
        }

        // Update / Delete: positive = rows affected, 0 = nothing changed, -1 = failed
        private string RowsResultText(int result)
        {
            if (result > 0)
                return result + " row(s) affected.";
            if (result == 0)
                return "No rows were affected (0).";
            return "Operation failed (-1): invalid data or a business rule was broken.";
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

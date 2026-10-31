using System;
using System.Web.Services.Protocols;
using System.Web.UI.WebControls;
using WebUsingOnLineSevices.MyDbServiceRef;

namespace WebUsingOnLineSevices.MyDbService
{
    // Button Click -> code behind -> SOAP proxy (MyDbServiceRef.WebService1) -> ASMX -> BLL -> DAL -> Access
    public partial class RolesPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRoles();
            }
        }

        // ===== GetAllRoles =====
        protected void btnGetAllRoles_Click(object sender, EventArgs e)
        {
            LoadRoles();
        }

        // One service call - the same list is shown in 3 different controls
        private void LoadRoles()
        {
            try
            {
                WebService1 service = new WebService1();
                Role[] roles = service.GetAllRoles();

                // A. ListBox - add the items one by one (Text = name, Value = ID)
                lstRoles.Items.Clear();
                foreach (Role r in roles)
                {
                    lstRoles.Items.Add(new ListItem(r.RoleName, r.RoleId.ToString()));
                }

                // B. Repeater - custom HTML from the ItemTemplate
                rptRoles.DataSource = roles;
                rptRoles.DataBind();

                // C. GridView - a table with explicit columns
                gvRoles.DataSource = roles;
                gvRoles.DataBind();

                lblGetAllRoles.Text = roles.Length == 0 ? "No roles found." : "Loaded " + roles.Length + " roles.";
            }
            catch (SoapException ex)
            {
                lblGetAllRoles.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblGetAllRoles.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== GetRoleById =====
        protected void btnGetRoleById_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtGetRoleId.Text, out id))
            {
                lblGetRoleById.Text = "Please enter a number.";
                return;
            }

            try
            {
                WebService1 service = new WebService1();
                Role role = service.GetRoleById(id);

                if (role == null)
                    lblGetRoleById.Text = "Role not found (null).";
                else
                    lblGetRoleById.Text = Server.HtmlEncode("ID: " + role.RoleId + ", Name: " + role.RoleName);
            }
            catch (SoapException ex)
            {
                lblGetRoleById.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblGetRoleById.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== AddRole =====
        protected void btnAddRole_Click(object sender, EventArgs e)
        {
            try
            {
                // Build the proxy object (generated from the WSDL)
                Role role = new Role();
                role.RoleName = txtAddRoleName.Text;

                WebService1 service = new WebService1();
                int result = service.AddRole(role);

                lblAddRole.Text = AddResultText(result);
                LoadRoles();
            }
            catch (SoapException ex)
            {
                lblAddRole.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblAddRole.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== UpdateRole =====
        protected void btnLoadSelectedRole_Click(object sender, EventArgs e)
        {
            if (lstRoles.SelectedItem == null)
            {
                lblUpdateRole.Text = "Please select a role in the ListBox.";
                return;
            }

            try
            {
                // The ListBox Value holds the role ID
                int id = int.Parse(lstRoles.SelectedValue);

                WebService1 service = new WebService1();
                Role role = service.GetRoleById(id);

                if (role == null)
                {
                    lblUpdateRole.Text = "Role not found.";
                    return;
                }

                txtEditRoleId.Text = role.RoleId.ToString();
                txtEditRoleName.Text = role.RoleName;
                lblUpdateRole.Text = "Role loaded. Change the name and click Update Role.";
            }
            catch (SoapException ex)
            {
                lblUpdateRole.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUpdateRole.Text = "Could not connect to the Web Service.";
            }
        }

        protected void btnUpdateRole_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtEditRoleId.Text, out id))
            {
                lblUpdateRole.Text = "Please load a role first.";
                return;
            }

            try
            {
                Role role = new Role();
                role.RoleId = id;
                role.RoleName = txtEditRoleName.Text;

                WebService1 service = new WebService1();
                int result = service.UpdateRole(role);

                lblUpdateRole.Text = RowsResultText(result);
                LoadRoles();
            }
            catch (SoapException ex)
            {
                lblUpdateRole.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblUpdateRole.Text = "Could not connect to the Web Service.";
            }
        }

        // ===== DeleteRole =====
        protected void btnDeleteRole_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(txtDeleteRoleId.Text, out id))
            {
                lblDeleteRole.Text = "Please enter a number.";
                return;
            }

            try
            {
                WebService1 service = new WebService1();
                int result = service.DeleteRole(id);

                lblDeleteRole.Text = RowsResultText(result);
                LoadRoles();
            }
            catch (SoapException ex)
            {
                lblDeleteRole.Text = SoapErrorText(ex);
            }
            catch (Exception)
            {
                lblDeleteRole.Text = "Could not connect to the Web Service.";
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

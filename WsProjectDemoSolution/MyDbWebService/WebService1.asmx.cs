using MyDbWebService.App_Code;
    using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace MyDbWebService
{
    /// <summary>
    /// שירות הרשת של הפרויקט.
    /// כל WebMethod מקבל פרמטרים, קורא ל-BLL ומחזיר את התוצאה.
    /// השירות לא ניגש ל-DAL או למסד הנתונים ישירות.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        #region Users
        // שמות המתודות הקיימות נשמרו, כדי לא לשבור לקוחות קיימים:
        // UserList = GetAllUsers, UserGetById = GetUserById, UserAdd = AddUser,
        // UserUpdate = UpdateUser, UserDelete = DeleteUser
        [WebMethod]
        public List<MyUser> UserList()
        {
            try
            {
                UserBLL bll = new UserBLL();
                return bll.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public MyUser UserGetById(int userid) {
            try
            {
                UserBLL bll = new UserBLL();
                return bll.GetUserById(userid);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public int UserAdd(MyUser u)
        {
            UserBLL bll = new UserBLL();
            return bll.AddUser(u);
        }
        [WebMethod]
        public int UserUpdate(MyUser u)
        {
            UserBLL bll = new UserBLL();
            return bll.UpdateUser(u);
        }
        [WebMethod]
        public int UserDelete(MyUser u)
        {
            UserBLL bll = new UserBLL();
            return bll.DeleteUser(u == null ? 0 : u.UserID);
        }
        [WebMethod]
        public bool ValidateUser(MyUser u)
        {
            try
            {
                UserBLL bll = new UserBLL();
                return bll.ValidateUser(u);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public MyUser UserByEmailPass(string userEmail, string userpass)
        {
            try
            {
                UserBLL bll = new UserBLL();
                return bll.UserByEmailPass(userEmail, userpass);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        #endregion

        #region Roles
        [WebMethod]
        public List<Role> GetAllRoles()
        {
            try
            {
                RoleBLL bll = new RoleBLL();
                return bll.GetAllRoles();
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public Role GetRoleById(int id)
        {
            try
            {
                RoleBLL bll = new RoleBLL();
                return bll.GetRoleById(id);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public int AddRole(Role r)
        {
            RoleBLL bll = new RoleBLL();
            return bll.AddRole(r);
        }
        [WebMethod]
        public int UpdateRole(Role r)
        {
            RoleBLL bll = new RoleBLL();
            return bll.UpdateRole(r);
        }
        [WebMethod]
        public int DeleteRole(int id)
        {
            RoleBLL bll = new RoleBLL();
            return bll.DeleteRole(id);
        }
        #endregion

        #region Articles
        // ArticleBLL זורק Exception כשהקלט לא תקין - כאן תופסים אותו (try-catch)
        [WebMethod]
        public List<Article> GetAllArticles()
        {
            try
            {
                ArticleBLL bll = new ArticleBLL();
                return bll.GetAllArticles();
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public Article GetArticleById(int id)
        {
            try
            {
                ArticleBLL bll = new ArticleBLL();
                return bll.GetArticleById(id);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public List<Article> GetArticlesByUser(int userId)
        {
            try
            {
                ArticleBLL bll = new ArticleBLL();
                return bll.GetArticlesByUser(userId);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public int AddArticle(Article a)
        {
            try
            {
                ArticleBLL bll = new ArticleBLL();
                return bll.AddArticle(a);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public int UpdateArticle(Article a)
        {
            try
            {
                ArticleBLL bll = new ArticleBLL();
                return bll.UpdateArticle(a);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        [WebMethod]
        public int DeleteArticle(int id)
        {
            try
            {
                ArticleBLL bll = new ArticleBLL();
                return bll.DeleteArticle(id);
            }
            catch (Exception ex)
            {
                throw ToSoapException(ex);
            }
        }
        #endregion

        // =========================
        // private helper
        // =========================
        // ממיר חריגה להודעה בטוחה ללקוח.
        // חריגה רגילה (new Exception) מה-BLL = קלט לא תקין => ההודעה נשלחת ללקוח.
        // כל חריגה אחרת (למשל שגיאת מסד נתונים) => הודעה כללית, בלי פרטים פנימיים.
        private SoapException ToSoapException(Exception ex)
        {
            if (ex.GetType() == typeof(Exception))
                return new SoapException(ex.Message, SoapException.ClientFaultCode);

            return new SoapException("אירעה שגיאה בשרת", SoapException.ServerFaultCode);
        }
    }
}

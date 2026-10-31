using MyDbWebService.App_Code;
    using System.Collections.Generic;
using System.Web.Services;

namespace MyDbWebService
{
    /// <summary>
    /// Summary description for WebService1
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

        #region USER
        [WebMethod]
        public MyUser UserGetById(int userid) {
            MyUserRepository repo = new MyUserRepository();
            return repo.GetById(userid);
        }
        [WebMethod]
        public int UserAdd(MyUser u)
        {
            MyUserRepository repo = new MyUserRepository();
            return repo.AddNew(u);
        }
        [WebMethod]
        public int UserUpdate(MyUser u)
        {
            MyUserRepository repo = new MyUserRepository();
            return repo.Update(u);
        }
        [WebMethod]
        public int UserDelete(MyUser u)
        {
            MyUserRepository repo = new MyUserRepository();
            return repo.Delete(u.UserID);
        }
        [WebMethod]
        public List<MyUser> UserList()
        {
            MyUserRepository repo = new MyUserRepository();
            return repo.GetList();
        }
        [WebMethod]
        public MyUser UserByEmailPass(string userEmail, string userpass)
        {
            MyUserRepository repo = new MyUserRepository();
            return repo.UserByEmailPass(userEmail, userpass);
        }
        #endregion
    }
}

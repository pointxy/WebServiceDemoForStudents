using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code
{
    /// <summary>
    /// שכבת הלוגיקה העסקית של תפקידים (Admin, Editor, User...).
    /// </summary>
    public class RoleBLL
    {
        private RoleRepository roleRepo = new RoleRepository();
        private MyUserRepository userRepo = new MyUserRepository();

        // =========================
        // Get
        // =========================
        public List<Role> GetAllRoles()
        {
            return roleRepo.GetList();
        }

        public Role GetRoleById(int id)
        {
            if (id <= 0)
                return null;

            return roleRepo.GetById(id);
        }

        // =========================
        // Add / Update / Delete
        // =========================
        // מחזיר את ה-ID החדש, או -1 אם נכשל
        public int AddRole(Role role)
        {
            try
            {
                if (role == null || string.IsNullOrWhiteSpace(role.RoleName))
                    return -1;

                return roleRepo.AddNew(role);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // מחזיר מספר שורות שעודכנו, או -1 אם שגיאה / התפקיד לא קיים
        public int UpdateRole(Role role)
        {
            try
            {
                if (role == null || role.RoleId <= 0)
                    return -1;

                if (string.IsNullOrWhiteSpace(role.RoleName))
                    return -1;

                if (roleRepo.GetById(role.RoleId) == null)
                    return -1;

                return roleRepo.Update(role);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // מחזיר מספר שורות שנמחקו, או -1 אם שגיאה / התפקיד לא קיים / יש לו משתמשים
        public int DeleteRole(int id)
        {
            try
            {
                if (id <= 0)
                    return -1;

                if (roleRepo.GetById(id) == null)
                    return -1;

                // אסור למחוק תפקיד שמשויך למשתמשים
                if (userRepo.GetByRoleId(id).Count > 0)
                    return -1;

                return roleRepo.Delete(id);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}

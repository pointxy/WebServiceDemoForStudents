using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code {

    public class MyUserRepository : IRepository<MyUser>
    {
        public int AddNew(MyUser user)
        {
            string sql = @"
            INSERT INTO tblUser 
            (firstName, lastName, userEmail, userPass, dob, roleId, isActive)
            VALUES
            (@firstName, @lastName, @email, @pass, @dob, @roleId, @isActive)";

            var parameters = new Dictionary<string, object>
        {
            {"@firstName", user.FirstName},
            {"@lastName", user.LastName},
            {"@email", user.UserEmail},
            {"@pass", user.UserPass},
            {"@dob", user.DOB},
            {"@roleId", user.UserRole.RoleId},
            {"@isActive", user.IsActive}
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public int Update(MyUser user)
        {
            // הסיסמה מתעדכנת רק אם נשלחה סיסמה חדשה
            bool updatePass = !string.IsNullOrEmpty(user.UserPass);

            string sql = @"
            UPDATE tblUser SET
                firstName = @firstName,
                lastName = @lastName,
                userEmail = @email,
                dob = @dob,
                roleId = @roleId,
                isActive = @isActive"
                + (updatePass ? ", userPass = @pass" : "") + @"
            WHERE userId = @id";

            // ב-Access הפרמטרים לפי סדר - חייב להתאים לסדר ב-SQL
            var parameters = new Dictionary<string, object>
        {
            {"@firstName", user.FirstName},
            {"@lastName", user.LastName},
            {"@email", user.UserEmail},
            {"@dob", user.DOB},
            {"@roleId", user.UserRole.RoleId},
            {"@isActive", user.IsActive}
        };
            if (updatePass)
                parameters.Add("@pass", user.UserPass);
            parameters.Add("@id", user.UserID);

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public int Delete(int id)
        {
            string sql = "DELETE FROM tblUser WHERE userId = @id";

            var parameters = new Dictionary<string, object>
        {
            {"@id", id}
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public MyUser GetById(int id)
        {
            string sql = @"
            SELECT u.*, r.roleName
            FROM tblUser u
            INNER JOIN tblRole r ON u.roleId = r.roleId
            WHERE u.userId = @id";

            var parameters = new Dictionary<string, object>
        {
            {"@id", id}
        };

            DataTable dt = DbHelper_Access.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count == 0)
                return null;

            return MapRowToUser(dt.Rows[0]);
        }

        public List<MyUser> GetList()
        {
            string sql = @"
            SELECT u.*, r.roleName
            FROM tblUser u
            INNER JOIN tblRole r ON u.roleId = r.roleId";

            DataTable dt = DbHelper_Access.ExecuteQuery(sql);

            List<MyUser> list = new List<MyUser>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToUser(row));
            }

            return list;
        }

        public MyUser UserByEmailPass(string userEmail, string userpass)
        {
            string sql = @"
            SELECT u.*, r.roleName
            FROM tblUser u
            INNER JOIN tblRole r ON u.roleId = r.roleId
            WHERE u.userEmail = @userEmail and u.userPass=@userPass";

            var parameters = new Dictionary<string, object>
        {
            {"@userEmail", userEmail},
            {"@userPass",userpass }
        };

            DataTable dt = DbHelper_Access.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count == 0)
                return null;

            return MapRowToUser(dt.Rows[0]);
        }

        public List<MyUser> GetByRoleId(int roleId)
        {
            string sql = @"
            SELECT u.*, r.roleName
            FROM tblUser u
            INNER JOIN tblRole r ON u.roleId = r.roleId
            WHERE u.roleId = @roleId";

            var parameters = new Dictionary<string, object>
        {
            {"@roleId", roleId}
        };

            DataTable dt = DbHelper_Access.ExecuteQuery(sql, parameters);

            List<MyUser> list = new List<MyUser>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToUser(row));
            }

            return list;
        }

        // =========================
        // private helper
        // =========================
        // הסיסמה לא נקראת כאן בכוונה - כדי שלא תישלח ללקוחות
        private MyUser MapRowToUser(DataRow row)
        {
            return new MyUser
            {
                UserID = Convert.ToInt32(row["userId"]),
                FirstName = row["firstName"].ToString(),
                LastName = row["lastName"].ToString(),
                UserEmail = row["userEmail"].ToString(),
                DOB = row["dob"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["dob"]),
                IsActive = Convert.ToBoolean(row["isActive"]),
                UserRole = new Role
                {
                    RoleId = Convert.ToInt32(row["roleId"]),
                    RoleName = row["roleName"].ToString()
                }
            };
        }
    }
}

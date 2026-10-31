using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code {
    public class RoleRepository : IRepository<Role>
    {
        public int AddNew(Role role)
        {
            string sql = @"
            INSERT INTO tblRole (roleName)
            VALUES (@roleName)";

            var parameters = new Dictionary<string, object>
        {
            { "@roleName", role.RoleName }
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public int Update(Role role)
        {
            string sql = @"
            UPDATE tblRole
            SET roleName = @roleName
            WHERE roleId = @id";

            var parameters = new Dictionary<string, object>
        {
            { "@roleName", role.RoleName },
            { "@id", role.RoleId }
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public int Delete(int id)
        {
            string sql = "DELETE FROM tblRole WHERE roleId = @id";

            var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public Role GetById(int id)
        {
            string sql = "SELECT * FROM tblRole WHERE roleId = @id";

            var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };

            DataTable dt = DbHelper_Access.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count == 0)
                return null;

            return MapRowToRole(dt.Rows[0]);
        }

        public List<Role> GetList()
        {
            string sql = "SELECT * FROM tblRole";

            DataTable dt = DbHelper_Access.ExecuteQuery(sql);

            List<Role> roles = new List<Role>();

            foreach (DataRow row in dt.Rows)
            {
                roles.Add(MapRowToRole(row));
            }

            return roles;
        }

        // =========================
        // private helper
        // =========================
        private Role MapRowToRole(DataRow row)
        {
            return new Role
            {
                RoleId = Convert.ToInt32(row["roleId"]),
                RoleName = row["roleName"].ToString()
            };
        }
    }

}
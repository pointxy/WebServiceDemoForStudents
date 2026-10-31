using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CommentsSrvice.App_Code
{
    public class SiteRepository : IRepository<Site>
    {
        public int AddNew(Site site)
        {
            string sql = @"
                INSERT INTO dbo.Sites
                    (SiteKey, PasswordHash, PasswordSalt, SiteName, IsActive)
                VALUES
                    (@SiteKey, @PasswordHash, @PasswordSalt, @SiteName, @IsActive)";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteKey", site.SiteKey},
                {"@PasswordHash", (object)site.PasswordHash ?? DBNull.Value},
                {"@PasswordSalt", (object)site.PasswordSalt ?? DBNull.Value},
                {"@SiteName", site.SiteName},
                {"@IsActive", site.IsActive}
            };

            // DbHelper_MsSql returns new ID for INSERT
            return DbHelper_MsSql.ExecuteNonQuery(sql, parameters);
        }

        public int Update(Site site)
        {
            string sql = @"
                UPDATE dbo.Sites SET
                    SiteKey = @SiteKey,
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    SiteName = @SiteName,
                    IsActive = @IsActive
                WHERE SiteId = @SiteId";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteKey", site.SiteKey},
                {"@PasswordHash", (object)site.PasswordHash ?? DBNull.Value},
                {"@PasswordSalt", (object)site.PasswordSalt ?? DBNull.Value},
                {"@SiteName", site.SiteName},
                {"@IsActive", site.IsActive},
                {"@SiteId", site.SiteId}
            };

            return DbHelper_MsSql.ExecuteNonQuery(sql, parameters);
        }

        public int Delete(int id)
        {
            // For demo: physical delete
            string sql = @"DELETE FROM dbo.Sites WHERE SiteId = @id";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return DbHelper_MsSql.ExecuteNonQuery(sql, parameters);
        }

        public Site GetById(int id)
        {
            string sql = @"SELECT * FROM dbo.Sites WHERE SiteId = @id";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count == 0) return null;

            return MapRowToSite(dt.Rows[0]);
        }

        public List<Site> GetList()
        {
            string sql = @"SELECT * FROM dbo.Sites ORDER BY SiteId";

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql);
            var list = new List<Site>();

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToSite(row));

            return list;
        }

        // Extra helper (useful for web service auth flow)
        public Site GetBySiteKey(string siteKey)
        {
            string sql = @"SELECT * FROM dbo.Sites WHERE SiteKey = @SiteKey";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteKey", siteKey}
            };

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count == 0) return null;

            return MapRowToSite(dt.Rows[0]);
        }

        // =========================
        // private helper
        // =========================
        private Site MapRowToSite(DataRow row)
        {
            return new Site
            {
                SiteId = Convert.ToInt32(row["SiteId"]),
                SiteKey = row["SiteKey"].ToString(),
                PasswordHash = row["PasswordHash"] == DBNull.Value ? null : (byte[])row["PasswordHash"],
                PasswordSalt = row["PasswordSalt"] == DBNull.Value ? null : (byte[])row["PasswordSalt"],
                SiteName = row["SiteName"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedAtUtc = Convert.ToDateTime(row["CreatedAtUtc"])
            };
        }
    }
}
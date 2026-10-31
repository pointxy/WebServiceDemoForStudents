using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code {

    public class ArticleRepository : IRepository<Article>
    {
        public int AddNew(Article article)
        {
            string sql = @"
            INSERT INTO tblArticle
            (caption, articleText, userId, publish, createDate)
            VALUES
            (@caption, @text, @userId, @publish, @createDate)";

            var parameters = new Dictionary<string, object>
        {
            { "@caption", article.Caption },
            { "@text", article.ArticleText },
            { "@userId", article.Author.UserID },
            { "@publish", article.Publish },
            { "@createDate", article.CreateDate }
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public int Update(Article article)
        {
            string sql = @"
            UPDATE tblArticle SET
                caption = @caption,
                articleText = @text,
                userId = @userId,
                publish = @publish
            WHERE articleId = @id";

            var parameters = new Dictionary<string, object>
        {
            { "@caption", article.Caption },
            { "@text", article.ArticleText },
            { "@userId", article.Author.UserID },
            { "@publish", article.Publish },
            { "@id", article.ArticleId }
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public int Delete(int id)
        {
            string sql = "DELETE FROM tblArticle WHERE articleId = @id";

            var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };

            return DbHelper_Access.ExecuteNonQuery(sql, parameters);
        }

        public Article GetById(int id)
        {
            string sql = @"
            SELECT a.*, 
                   u.firstName, u.lastName, u.userEmail, u.dob, u.isActive,
                   r.roleId, r.roleName
            FROM (tblArticle a
            INNER JOIN tblUser u ON a.userId = u.userId)
            INNER JOIN tblRole r ON u.roleId = r.roleId
            WHERE a.articleId = @id";

            var parameters = new Dictionary<string, object>
        {
            { "@id", id }
        };

            DataTable dt = DbHelper_Access.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count == 0)
                return null;

            return MapRowToArticle(dt.Rows[0]);
        }

        public List<Article> GetList()
        {
            string sql = @"
            SELECT a.*, 
                   u.firstName, u.lastName, u.userEmail, u.dob, u.isActive,
                   r.roleId, r.roleName
            FROM (tblArticle a
            INNER JOIN tblUser u ON a.userId = u.userId)
            INNER JOIN tblRole r ON u.roleId = r.roleId";

            DataTable dt = DbHelper_Access.ExecuteQuery(sql);

            List<Article> list = new List<Article>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToArticle(row));
            }

            return list;
        }

        public List<Article> GetByUserId(int userId)
        {
            string sql = @"
            SELECT a.*,
                   u.firstName, u.lastName, u.userEmail, u.dob, u.isActive,
                   r.roleId, r.roleName
            FROM (tblArticle a
            INNER JOIN tblUser u ON a.userId = u.userId)
            INNER JOIN tblRole r ON u.roleId = r.roleId
            WHERE a.userId = @userId";

            var parameters = new Dictionary<string, object>
        {
            { "@userId", userId }
        };

            DataTable dt = DbHelper_Access.ExecuteQuery(sql, parameters);

            List<Article> list = new List<Article>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToArticle(row));
            }

            return list;
        }

        // =========================
        // private helper
        // =========================
        private Article MapRowToArticle(DataRow row)
        {
            return new Article
            {
                ArticleId = Convert.ToInt32(row["articleId"]),
                Caption = row["caption"].ToString(),
                ArticleText = row["articleText"].ToString(),
                Publish = Convert.ToBoolean(row["publish"]),
                CreateDate = Convert.ToDateTime(row["createDate"]),
                Author = new MyUser
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
                }
            };
        }
    }

}
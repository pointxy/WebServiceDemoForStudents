using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CommentsSrvice.App_Code
{

    public class CommentRepository : IRepository<Comment>
    {
        public int AddNew(Comment c)
        {
            string sql = @"
                INSERT INTO dbo.Comments
                    (SiteId, ExternalContentId, ParentCommentId, Title, Body,
                     AuthorName, AuthorEmail, IsApproved, IsDeleted)
                VALUES
                    (@SiteId, @ExternalContentId, @ParentCommentId, @Title, @Body,
                     @AuthorName, @AuthorEmail, @IsApproved, @IsDeleted)";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteId", c.SiteId},
                {"@ExternalContentId", c.ExternalContentId},
                {"@ParentCommentId", c.ParentCommentId},
                {"@Title", c.Title},
                {"@Body", c.Body},
                {"@AuthorName", (object)c.AuthorName ?? DBNull.Value},
                {"@AuthorEmail", (object)c.AuthorEmail ?? DBNull.Value},
                {"@IsApproved", c.IsApproved},
                {"@IsDeleted", c.IsDeleted}
            };

            return DbHelper_MsSql.ExecuteNonQuery(sql, parameters);
        }

        public int Update(Comment c)
        {
            string sql = @"
                UPDATE dbo.Comments SET
                    ExternalContentId = @ExternalContentId,
                    ParentCommentId = @ParentCommentId,
                    Title = @Title,
                    Body = @Body,
                    AuthorName = @AuthorName,
                    AuthorEmail = @AuthorEmail,
                    IsApproved = @IsApproved,
                    IsDeleted = @IsDeleted,
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE CommentId = @CommentId AND SiteId = @SiteId";

            var parameters = new Dictionary<string, object>
            {
                {"@ExternalContentId", c.ExternalContentId},
                {"@ParentCommentId", c.ParentCommentId},
                {"@Title", c.Title},
                {"@Body", c.Body},
                {"@AuthorName", (object)c.AuthorName ?? DBNull.Value},
                {"@AuthorEmail", (object)c.AuthorEmail ?? DBNull.Value},
                {"@IsApproved", c.IsApproved},
                {"@IsDeleted", c.IsDeleted},
                {"@CommentId", c.CommentId},
                {"@SiteId", c.SiteId}
            };

            return DbHelper_MsSql.ExecuteNonQuery(sql, parameters);
        }

        public int Delete(int id)
        {
            // soft delete (recommended for comments)
            string sql = @"
                UPDATE dbo.Comments SET
                    IsDeleted = 1,
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE CommentId = @id";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return DbHelper_MsSql.ExecuteNonQuery(sql, parameters);
        }

        public Comment GetById(int id)
        {
            string sql = @"SELECT * FROM dbo.Comments WHERE CommentId = @id";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql, parameters);
            if (dt.Rows.Count == 0) return null;

            return MapRowToComment(dt.Rows[0]);
        }

        public List<Comment> GetList()
        {
            // All comments (admin-style)
            string sql = @"SELECT * FROM dbo.Comments ORDER BY CreatedAtUtc";

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql);
            var list = new List<Comment>();

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToComment(row));

            return list;
        }

        // =========================
        // Useful extra methods
        // =========================

        // Public view: approved + not deleted, for a content item
        public List<Comment> GetForContent(int siteId, string externalContentId)
        {
            string sql = @"
                SELECT *
                FROM dbo.Comments
                WHERE SiteId = @SiteId
                  AND ExternalContentId = @ExternalContentId
                  AND IsApproved = 1
                  AND IsDeleted = 0
                ORDER BY CreatedAtUtc";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteId", siteId},
                {"@ExternalContentId", externalContentId}
            };

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql, parameters);
            var list = new List<Comment>();

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToComment(row));

            return list;
        }

        public List<Comment> GetRootComments(int siteId, string externalContentId)
        {
            string sql = @"
                SELECT *
                FROM dbo.Comments
                WHERE SiteId = @SiteId
                  AND ExternalContentId = @ExternalContentId
                  AND ParentCommentId = 0
                  AND IsApproved = 1
                  AND IsDeleted = 0
                ORDER BY CreatedAtUtc";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteId", siteId},
                {"@ExternalContentId", externalContentId}
            };

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql, parameters);
            var list = new List<Comment>();

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToComment(row));

            return list;
        }

        public List<Comment> GetReplies(int siteId, string externalContentId, int parentCommentId)
        {
            string sql = @"
                SELECT *
                FROM dbo.Comments
                WHERE SiteId = @SiteId
                  AND ExternalContentId = @ExternalContentId
                  AND ParentCommentId = @ParentCommentId
                  AND IsApproved = 1
                  AND IsDeleted = 0
                ORDER BY CreatedAtUtc";

            var parameters = new Dictionary<string, object>
            {
                {"@SiteId", siteId},
                {"@ExternalContentId", externalContentId},
                {"@ParentCommentId", parentCommentId}
            };

            DataTable dt = DbHelper_MsSql.ExecuteQuery(sql, parameters);
            var list = new List<Comment>();

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToComment(row));

            return list;
        }

        // =========================
        // private helper
        // =========================
        private Comment MapRowToComment(DataRow row)
        {
            return new Comment
            {
                CommentId = Convert.ToInt32(row["CommentId"]),
                SiteId = Convert.ToInt32(row["SiteId"]),
                ExternalContentId = row["ExternalContentId"].ToString(),
                ParentCommentId = Convert.ToInt32(row["ParentCommentId"]),
                Title = row["Title"].ToString(),
                Body = row["Body"].ToString(),
                AuthorName = row["AuthorName"] == DBNull.Value ? null : row["AuthorName"].ToString(),
                AuthorEmail = row["AuthorEmail"] == DBNull.Value ? null : row["AuthorEmail"].ToString(),
                IsApproved = Convert.ToBoolean(row["IsApproved"]),
                IsDeleted = Convert.ToBoolean(row["IsDeleted"]),
                CreatedAtUtc = Convert.ToDateTime(row["CreatedAtUtc"]),
                UpdatedAtUtc = row["UpdatedAtUtc"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["UpdatedAtUtc"])
            };
        }
    }
}


using CommentsSrvice.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;

namespace CommentsSrvice
{
    /// <summary>
    /// Summary description for CommentService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CommentService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Comments Service is running";
        }

        // =========================================================
        // AUTH HELPERS (SiteKey + Password)
        // =========================================================
        private Site AuthenticateSite(string siteKey, string sitePassword)
        {
            SiteRepository repo = new SiteRepository();
            Site site = repo.GetBySiteKey(siteKey);

            if (site == null) return null;
            if (!site.IsActive) return null;

            if (!VerifyPassword(sitePassword, site.PasswordSalt, site.PasswordHash))
                return null;

            return site;
        }

        private static bool VerifyPassword(string password, byte[] salt, byte[] expectedHash)
        {
            if (salt == null || expectedHash == null) return false;

            byte[] actualHash = ComputeHash(password, salt, expectedHash.Length);
            if (actualHash == null || actualHash.Length != expectedHash.Length) return false;

            // constant-time compare
            int diff = 0;
            for (int i = 0; i < expectedHash.Length; i++)
                diff |= (expectedHash[i] ^ actualHash[i]);

            return diff == 0;
        }

        private static byte[] ComputeHash(string password, byte[] salt, int hashLen)
        {
            if (password == null) password = "";
            byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(password);

            byte[] input = new byte[salt.Length + passBytes.Length];
            Buffer.BlockCopy(salt, 0, input, 0, salt.Length);
            Buffer.BlockCopy(passBytes, 0, input, salt.Length, passBytes.Length);

            // Choose SHA256 (32 bytes) or SHA512 (64 bytes) based on expectedHash length
            if (hashLen <= 32)
            {
                using (SHA256 sha = SHA256.Create())
                    return sha.ComputeHash(input);
            }
            else
            {
                using (SHA512 sha = SHA512.Create())
                    return sha.ComputeHash(input);
            }
        }

        private static byte[] CreateSalt(int sizeBytes = 16)
        {
            byte[] salt = new byte[sizeBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // =========================================================
        // SITE
        // =========================================================
        #region SITE

        [WebMethod(Description = "Register a new site. Returns new SiteId.")]
        public int SiteRegister(string siteKey, string sitePassword, string siteName)
        {
            if (string.IsNullOrWhiteSpace(siteKey)) return -1;
            if (string.IsNullOrWhiteSpace(sitePassword)) return -2;
            if (string.IsNullOrWhiteSpace(siteName)) return -3;

            SiteRepository repo = new SiteRepository();

            // prevent duplicates
            Site existing = repo.GetBySiteKey(siteKey);
            if (existing != null) return -4;

            byte[] salt = CreateSalt(16);

            // Hash length: match your DB choice.
            // If you store VARBINARY(64) use SHA512; if VARBINARY(32) use SHA256.
            // Here we'll store SHA256 by default (32 bytes).
            byte[] hash = ComputeHash(sitePassword, salt, 32);

            Site s = new Site
            {
                SiteKey = siteKey.Trim(),
                SiteName = siteName.Trim(),
                PasswordSalt = salt,
                PasswordHash = hash,
                IsActive = true
            };

            return repo.AddNew(s); // returns new SiteId (per your DbHelper style)
        }

        [WebMethod(Description = "Login check. Returns Site object if ok, else null.")]
        public Site SiteLogin(string siteKey, string sitePassword)
        {
            return AuthenticateSite(siteKey, sitePassword);
        }

        [WebMethod(Description = "Get site by key (requires password).")]
        public Site SiteGet(string siteKey, string sitePassword)
        {
            return AuthenticateSite(siteKey, sitePassword);
        }

        [WebMethod(Description = "List all sites (DEMO/admin).")]
        public List<Site> SiteList()
        {
            SiteRepository repo = new SiteRepository();
            return repo.GetList();
        }

        #endregion

        // =========================================================
        // COMMENTS
        // =========================================================
        #region COMMENTS

        [WebMethod(Description = "Add a comment. parentCommentId=0 => root. Returns new CommentId.")]
        public int CommentAdd(
            string siteKey,
            string sitePassword,
            string externalContentId,
            int parentCommentId,
            string title,
            string body,
            string authorName,
            string authorEmail)
        {
            Site site = AuthenticateSite(siteKey, sitePassword);
            if (site == null) return -10;

            if (string.IsNullOrWhiteSpace(externalContentId)) return -11;
            if (string.IsNullOrWhiteSpace(title)) return -12;
            if (string.IsNullOrWhiteSpace(body)) return -13;
            if (parentCommentId < 0) return -14;

            CommentRepository repo = new CommentRepository();

            Comment c = new Comment
            {
                SiteId = site.SiteId,
                ExternalContentId = externalContentId.Trim(),
                ParentCommentId = parentCommentId,
                Title = title.Trim(),
                Body = body,
                AuthorName = string.IsNullOrWhiteSpace(authorName) ? null : authorName.Trim(),
                AuthorEmail = string.IsNullOrWhiteSpace(authorEmail) ? null : authorEmail.Trim(),
                IsApproved = true,   // demo default
                IsDeleted = false
            };

            // DB trigger will validate ParentCommentId belongs to same SiteId + ExternalContentId
            return repo.AddNew(c);
        }

        [WebMethod(Description = "Get all public comments for a content item (approved + not deleted).")]
        public List<Comment> CommentGetForContent(string siteKey, string externalContentId)
        {
            SiteRepository siteRepo = new SiteRepository();
            Site site = siteRepo.GetBySiteKey(siteKey);
            if (site == null || !site.IsActive) return new List<Comment>();

            CommentRepository repo = new CommentRepository();
            return repo.GetForContent(site.SiteId, externalContentId);
        }

        [WebMethod(Description = "Get root comments for content (ParentCommentId=0).")]
        public List<Comment> CommentGetRoots(string siteKey, string externalContentId)
        {
            SiteRepository siteRepo = new SiteRepository();
            Site site = siteRepo.GetBySiteKey(siteKey);
            if (site == null || !site.IsActive) return new List<Comment>();

            CommentRepository repo = new CommentRepository();
            return repo.GetRootComments(site.SiteId, externalContentId);
        }

        [WebMethod(Description = "Get replies for a parent comment.")]
        public List<Comment> CommentGetReplies(string siteKey, string externalContentId, int parentCommentId)
        {
            SiteRepository siteRepo = new SiteRepository();
            Site site = siteRepo.GetBySiteKey(siteKey);
            if (site == null || !site.IsActive) return new List<Comment>();

            CommentRepository repo = new CommentRepository();
            return repo.GetReplies(site.SiteId, externalContentId, parentCommentId);
        }

        // -------------------------
        // Admin / moderation (requires password)
        // -------------------------

        [WebMethod(Description = "Approve/Unapprove comment (admin). Returns 1 if ok, 0 otherwise.")]
        public int CommentSetApproval(string siteKey, string sitePassword, int commentId, bool isApproved)
        {
            Site site = AuthenticateSite(siteKey, sitePassword);
            if (site == null) return 0;

            CommentRepository repo = new CommentRepository();
            Comment c = repo.GetById(commentId);
            if (c == null) return 0;
            if (c.SiteId != site.SiteId) return 0;

            c.IsApproved = isApproved;
            return repo.Update(c);
        }

        [WebMethod(Description = "Soft delete comment (admin). Returns 1 if ok, 0 otherwise.")]
        public int CommentSoftDelete(string siteKey, string sitePassword, int commentId)
        {
            Site site = AuthenticateSite(siteKey, sitePassword);
            if (site == null) return 0;

            CommentRepository repo = new CommentRepository();
            Comment c = repo.GetById(commentId);
            if (c == null) return 0;
            if (c.SiteId != site.SiteId) return 0;

            // repository Delete() is soft-delete in our demo
            return repo.Delete(commentId);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentsSrvice.App_Code
{
    public class Site
    {
        public int SiteId { get; set; }
        public string SiteKey { get; set; }

        // Store as bytes (VARBINARY in DB)
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string SiteName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
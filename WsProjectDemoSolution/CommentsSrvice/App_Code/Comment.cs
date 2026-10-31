using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentsSrvice.App_Code
{

    public class Comment
    {
        public int CommentId { get; set; }            // demo uses int (DB can be BIGINT)
        public int SiteId { get; set; }

        public string ExternalContentId { get; set; } // e.g. "Article-100"
        public int ParentCommentId { get; set; }      // 0 = root

        public string Title { get; set; }
        public string Body { get; set; }

        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }

        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}


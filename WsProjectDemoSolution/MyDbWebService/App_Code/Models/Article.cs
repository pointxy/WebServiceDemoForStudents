using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Caption { get; set; }
        public string ArticleText { get; set; }
        public MyUser Author { get; set; }
        public bool Publish { get; set; }
        public DateTime CreateDate { get; set; }
    }

}
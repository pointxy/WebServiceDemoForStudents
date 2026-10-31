using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code {
    public class MyUser
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public   DateTime DOB { get; set; }
        public bool IsActive { get; set; }
        public Role UserRole { get; set; }
    }
}
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
        public string UserPass { get; set; }   // נשמר במסד, לא מוחזר בשליפות
        public   DateTime? DOB { get; set; }   // ריק מותר במסד
        public bool IsActive { get; set; }
        public Role UserRole { get; set; }
    }
}
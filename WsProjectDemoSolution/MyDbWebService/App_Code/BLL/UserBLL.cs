using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code
{
    /// <summary>
    /// שכבת הלוגיקה העסקית של משתמשים.
    /// בודקת את הנתונים ואת הכללים העסקיים, ורק אז פונה ל-Repository.
    /// </summary>
    public class UserBLL
    {
        private MyUserRepository userRepo = new MyUserRepository();
        private RoleRepository roleRepo = new RoleRepository();
        private ArticleRepository articleRepo = new ArticleRepository();

        // =========================
        // Get
        // =========================
        public List<MyUser> GetAllUsers()
        {
            return userRepo.GetList();
        }

        public MyUser GetUserById(int id)
        {
            if (id <= 0)
                return null;

            return userRepo.GetById(id);
        }

        // התחברות - נשמרת כמו שהייתה (אימייל + סיסמה)
        public MyUser UserByEmailPass(string userEmail, string userPass)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrEmpty(userPass))
                return null;

            return userRepo.UserByEmailPass(userEmail.Trim(), userPass);
        }

        // =========================
        // Validate
        // =========================
        // בודק שנתוני המשתמש תקינים (זו לא פעולת התחברות)
        public bool ValidateUser(MyUser user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(user.FirstName))
                return false;

            if (string.IsNullOrWhiteSpace(user.LastName))
                return false;

            if (string.IsNullOrWhiteSpace(user.UserEmail) || !user.UserEmail.Contains("@"))
                return false;

            // התפקיד חייב להיות קיים בטבלת התפקידים
            if (user.UserRole == null || user.UserRole.RoleId <= 0)
                return false;

            if (roleRepo.GetById(user.UserRole.RoleId) == null)
                return false;

            return true;
        }

        // =========================
        // Add / Update / Delete
        // =========================
        // מחזיר את ה-ID החדש, או -1 אם נכשל
        public int AddUser(MyUser user)
        {
            try
            {
                if (!ValidateUser(user))
                    return -1;

                // למשתמש חדש חובה סיסמה
                if (string.IsNullOrEmpty(user.UserPass))
                    return -1;

                if (EmailExists(user.UserEmail, 0))
                    return -1;

                return userRepo.AddNew(user);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // מחזיר מספר שורות שעודכנו, 0 אם לא נמצא, -1 אם שגיאה
        public int UpdateUser(MyUser user)
        {
            try
            {
                if (user == null || user.UserID <= 0)
                    return -1;

                if (!ValidateUser(user))
                    return -1;

                if (EmailExists(user.UserEmail, user.UserID))
                    return -1;

                return userRepo.Update(user);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // מחזיר מספר שורות שנמחקו, 0 אם לא נמצא, -1 אם שגיאה
        public int DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                    return -1;

                // אסור למחוק משתמש שכתב מאמרים
                if (articleRepo.GetByUserId(id).Count > 0)
                    return -1;

                return userRepo.Delete(id);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // =========================
        // private helper
        // =========================
        // בודק אם האימייל כבר שייך למשתמש אחר (חוץ מ-exceptUserId)
        private bool EmailExists(string email, int exceptUserId)
        {
            string e = email.Trim().ToLower();

            return userRepo.GetList()
                .Any(u => u.UserID != exceptUserId && u.UserEmail.Trim().ToLower() == e);
        }
    }
}

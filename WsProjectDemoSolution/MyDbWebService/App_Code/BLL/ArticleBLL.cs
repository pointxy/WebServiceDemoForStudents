using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDbWebService.App_Code
{
    /// <summary>
    /// שכבת הלוגיקה העסקית של מאמרים.
    /// כאן מודגמת זריקת חריגה (throw new Exception) כשהקלט לא תקין.
    /// השירות (ASMX) תופס את החריגה ומחזיר ללקוח את ההודעה.
    /// </summary>
    public class ArticleBLL
    {
        private ArticleRepository articleRepo = new ArticleRepository();
        private MyUserRepository userRepo = new MyUserRepository();

        // =========================
        // Get
        // =========================
        public List<Article> GetAllArticles()
        {
            return articleRepo.GetList();
        }

        public Article GetArticleById(int id)
        {
            if (id <= 0)
                throw new Exception("מזהה מאמר לא תקין");

            return articleRepo.GetById(id);
        }

        public List<Article> GetArticlesByUser(int userId)
        {
            if (userId <= 0)
                throw new Exception("מזהה משתמש לא תקין");

            return articleRepo.GetByUserId(userId);
        }

        // =========================
        // Add / Update / Delete
        // =========================
        // מחזיר את ה-ID החדש, או -1 אם שמירה במסד נכשלה
        // קלט לא תקין => זורק חריגה
        public int AddArticle(Article article)
        {
            CheckArticle(article);

            // תאריך יצירה - אם לא נשלח, התאריך של היום
            if (article.CreateDate == DateTime.MinValue)
                article.CreateDate = DateTime.Now;

            try
            {
                return articleRepo.AddNew(article);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // מחזיר מספר שורות שעודכנו, או -1 אם שמירה במסד נכשלה
        public int UpdateArticle(Article article)
        {
            if (article != null && article.ArticleId <= 0)
                throw new Exception("מזהה מאמר לא תקין");

            CheckArticle(article);

            if (articleRepo.GetById(article.ArticleId) == null)
                throw new Exception("המאמר לא קיים");

            try
            {
                return articleRepo.Update(article);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // מחזיר מספר שורות שנמחקו, או -1 אם שמירה במסד נכשלה
        public int DeleteArticle(int id)
        {
            if (id <= 0)
                throw new Exception("מזהה מאמר לא תקין");

            if (articleRepo.GetById(id) == null)
                throw new Exception("המאמר לא קיים");

            try
            {
                return articleRepo.Delete(id);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // =========================
        // private helper
        // =========================
        // בדיקות קלט משותפות להוספה ולעדכון
        private void CheckArticle(Article article)
        {
            if (article == null)
                throw new Exception("לא התקבל מאמר");

            if (string.IsNullOrWhiteSpace(article.Caption))
                throw new Exception("חובה להזין כותרת למאמר");

            if (string.IsNullOrWhiteSpace(article.ArticleText))
                throw new Exception("חובה להזין תוכן למאמר");

            if (article.Author == null || article.Author.UserID <= 0)
                throw new Exception("חובה לציין מחבר למאמר");

            if (userRepo.GetById(article.Author.UserID) == null)
                throw new Exception("המחבר לא קיים");
        }
    }
}

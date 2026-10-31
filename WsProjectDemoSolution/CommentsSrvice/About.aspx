<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="CommentsSrvice.About" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl">
<head runat="server">
    <title>About - Comments Web Service</title>
    <style>
        body {
            font-family: Arial;
            margin: 20px;
            line-height: 1.6;
        }

        .card {
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 16px;
            margin-bottom: 16px;
           
        }

        h1, h2 {
            margin-top: 0;
        }

        code, pre {
            background: #f6f6f6;
            border: 1px solid #e1e1e1;
            border-radius: 8px;
            
        }

        pre {
            padding: 12px;
            overflow: auto;
           direction:ltr;
        }

        .small {
            color: #666;
            font-size: 0.95em;
        }

        .tag {
            display: inline-block;
            padding: 2px 8px;
            border-radius: 999px;
            background: #eef;
            margin-left: 6px;
            font-size: 0.9em;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th, td {
            border: 1px solid #ddd;
            padding: 8px;
            vertical-align: top;
        }

        th {
            background: #fafafa;
        }

        .warn {
            background: #fff7e6;
            border: 1px solid #ffe1a6;
            padding: 10px;
            border-radius: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <h1>About – Comments Web Service</h1>
        <p class="small">
            שירות רשת (XML Web Service – ASMX) שמספק <b>מערכת תגובות מרכזית</b> לאתרים שונים.
            כל אתר מקבל <b>SiteKey</b> וסיסמה, ושומר תגובות לפי מזהה תוכן חיצוני (ExternalContentId).
        </p>

        <div class="card">
            <h2>מה השירות נותן?</h2>
            <ul>
                <li><b>תגובות לפי תוכן</b> – כל תגובה שייכת ל־ExternalContentId (למשל Article-100).</li>
                <li><b>תגובות משנה</b> – כל תגובה יכולה להיות תגובה לתגובה אחרת (עץ תגובות).</li>
                <li><b>הפרדה בין אתרים</b> – אתר רואה ומנהל רק את התגובות שלו.</li>
                <li><b>ניהול תגובות</b> – אישור/ביטול אישור ומחיקה לוגית (Soft Delete) לפי הרשאות.</li>
            </ul>
            <div class="warn">
                <b>כלל חשוב:</b> תגובה ראשית מסומנת עם <b>ParentCommentId = 0</b>.
            </div>
        </div>

        <div class="card">
            <h2>איך מתחברים?</h2>
            <ol>
                <li>נרשמים פעם אחת באמצעות הפעולה <b>SiteRegister</b> ומקבלים אתר במערכת.</li>
                <li>בכל פעולה ניהולית/כתיבה שולחים <b>SiteKey + SitePassword</b>.</li>
                <li>פעולות קריאה ציבוריות (תצוגה באתר) בדרך כלל דורשות רק <b>SiteKey</b>.</li>
            </ol>

            <h3>דוגמאות פרמטרים</h3>
            <table>
                <tr>
                    <th>שם</th>
                    <th>דוגמה</th>
                    <th>הסבר</th>
                </tr>
                <tr>
                    <td>SiteKey</td>
                    <td><code>siteA</code></td>
                    <td>מפתח ייחודי של האתר</td>
                </tr>
                <tr>
                    <td>SitePassword</td>
                    <td><code>1234</code></td>
                    <td>סיסמה של האתר לפעולות כתיבה/ניהול</td>
                </tr>
                <tr>
                    <td>ExternalContentId</td>
                    <td><code>Article-100</code></td>
                    <td>מזהה התוכן באתר הלקוח (כתבה/מוצר/עמוד)</td>
                </tr>
                <tr>
                    <td>ParentCommentId</td>
                    <td><code>0</code> או <code>15</code></td>
                    <td>0 לתגובה ראשית, או מזהה תגובה קיימת לתגובת משנה</td>
                </tr>
            </table>
        </div>

        <div class="card">
            <h2>רשימת פעולות (WebMethods)</h2>
            <p class="small">
                השירות נכתב בקובץ <b>CommentService.asmx</b>. להלן הפעולות העיקריות:
            </p>

            <h3>Site</h3>
            <ul>
                <li><b>HelloWorld()</b> – בדיקה שהשירות עובד.</li>
                <li><b>SiteRegister(siteKey, sitePassword, siteName)</b> – רישום אתר חדש.</li>
                <li><b>SiteLogin(siteKey, sitePassword)</b> – בדיקת כניסה/אימות.</li>
                <li><b>SiteGet(siteKey, sitePassword)</b> – קבלת פרטי אתר (דורש סיסמה).</li>
                <li><b>SiteList()</b> – רשימת אתרים (דמו/אדמין).</li>
            </ul>

            <h3>Comments</h3>
            <ul>
                <li><b>CommentAdd(siteKey, sitePassword, externalContentId, parentCommentId, title, body, authorName, authorEmail)</b></li>
                <li><b>CommentGetForContent(siteKey, externalContentId)</b> – כל התגובות הציבוריות לתוכן (מאושרות ולא מחוקות).</li>
                <li><b>CommentGetRoots(siteKey, externalContentId)</b> – תגובות ראשיות בלבד (Parent=0).</li>
                <li><b>CommentGetReplies(siteKey, externalContentId, parentCommentId)</b> – תגובות־ילדים לתגובה מסוימת.</li>
                <li><b>CommentSetApproval(siteKey, sitePassword, commentId, isApproved)</b> – ניהול אישור תגובה.</li>
                <li><b>CommentSoftDelete(siteKey, sitePassword, commentId)</b> – מחיקה לוגית.</li>
            </ul>
        </div>

        <div class="card">
            <h2>דוגמה מהירה לשימוש באתר לקוח</h2>

            <h3>1) הוספת Web Reference</h3>
            <p>
                ב־Visual Studio: קליק ימני על הפרויקט → <b>Add Service Reference</b> או <b>Add Web Reference</b>
                → הדבקת ה־URL של השירות (למשל):
            </p>
            <pre>http://localhost:port/CommentService.asmx</pre>

            <h3>2) קריאה לדוגמה (רעיון)</h3>
            <p class="small">
                לאחר הוספת רפרנס, תוכל ליצור אובייקט Proxy ולקרוא לפעולות.
                (השם המדויק של המחלקה תלוי בשם ה־Web Reference שבחרת.)
            </p>
            <pre>
// pseudo example:
// var svc = new CommentService();
// var roots = svc.CommentGetRoots("siteA", "Article-100");
            </pre>

            <h3>3) הוספת תגובה ראשית</h3>
            <pre>
// parentCommentId = 0 means root comment
// svc.CommentAdd("siteA","1234","Article-100",0,"Title","Body","Dan","dan@mail.com");
            </pre>

            <h3>4) הוספת תגובת משנה</h3>
            <pre>
// reply to existing commentId=15
// svc.CommentAdd("siteA","1234","Article-100",15,"Re: ...","My reply", "Dana", null);
            </pre>
        </div>

        <div class="card">
            <h2>קישורים שימושיים</h2>
            <ul>
                <li>קובץ השירות: <b>CommentService.asmx</b></li>
                <li>DB: טבלאות <b>Sites</b>, <b>Comments</b></li>
            </ul>
            <p class="small">
                זהו דף הסבר/תיעוד (About) לתלמידים ולמשתמשים בשירות.
            </p>
        </div>

    </form>
</body>
</html>

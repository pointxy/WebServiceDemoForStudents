# ניתוח פערים – BLL ו-Web Service (שלב 2)

פרויקט: `MyDbWebService` (Web Application Project, ‏.NET Framework 4.8). תאריך: 2026-10-31.
ניתוח בלבד – לא בוצעו שינויים בקוד, בפרויקט או בבסיס הנתונים. רקע כללי: `ExistingSystem.md`.
מקרא: ✅ אומת בקוד/DB · ⚠️ הסקה/לא נבדק בהרצה.

## 1. סיכום המימוש הקיים

| רכיב | קיים | הערות |
|---|---|---|
| Models | `MyUser`, `Role`, `Article` ✅ | `Article.Caption` = כותרת, `Article.ArticleText` = תוכן, `Article.Author` = `MyUser` |
| Repositories | `RoleRepository`, `MyUserRepository`, `ArticleRepository` – כל אחד מממש `IRepository<T>` (GetById, AddNew, Update, Delete, GetList) ✅ | ב-`MyUserRepository` יש גם `UserByEmailPass` |
| DbHelper | `DbHelper_Access.ExecuteQuery` / `ExecuteNonQuery` ✅ | שאילתות עם פרמטרים, אין try/catch (חריגות עוברות למעלה) |
| BLL | התיקייה `App_Code/BLL` קיימת אבל ריקה ✅ | — |
| Web Service | `WebService1.asmx` עם מתודות User בלבד, שקוראות ל-DAL ישירות ✅ | **עוקף את שכבת ה-BLL** |

## 2. תכנון ה-BLL המאושר ומיפוי למתודות קיימות

| BLL | מתודה | מתודה קיימת לשימוש | מצב |
|---|---|---|---|
| UserBLL | GetAllUsers | `MyUserRepository.GetList` | ✅ ניתן לממש |
| | GetUserById | `MyUserRepository.GetById` | ✅ |
| | AddUser | `MyUserRepository.AddNew` | ✅ (ראו פער P1 – סיסמה) |
| | UpdateUser | `MyUserRepository.Update` | ✅ |
| | DeleteUser | `MyUserRepository.Delete` + בדיקת מאמרים | ⚠️ צריך את GetArticlesByUser (G1) |
| | ValidateUser | אין – לוגיקה טהורה + `RoleRepository.GetById` | ✅ |
| RoleBLL | GetAllRoles / GetRoleById / AddRole / UpdateRole | `RoleRepository` | ✅ |
| | DeleteRole | `RoleRepository.Delete` + בדיקת משתמשים | ⚠️ צריך משתמשים לפי תפקיד (G2) |
| ArticleBLL | GetAllArticles / GetArticleById / AddArticle / UpdateArticle / DeleteArticle | `ArticleRepository` | ✅ |
| | GetArticlesByUser | **אין** | ❌ G1 |

## 3. מתודות Web Service נדרשות

שירות ASMX אחד, מחולק ל-`#region Users`, `#region Roles`, `#region Articles`. כל מתודה קוראת ל-BLL בלבד.

| Region | WebMethods (מוצע – שמות זהים ל-BLL) |
|---|---|
| Users | `GetAllUsers()`, `GetUserById(int id)`, `AddUser(MyUser u)`, `UpdateUser(MyUser u)`, `DeleteUser(int id)`, `ValidateUser(MyUser u)` |
| Roles | `GetAllRoles()`, `GetRoleById(int id)`, `AddRole(Role r)`, `UpdateRole(Role r)`, `DeleteRole(int id)` |
| Articles | `GetAllArticles()`, `GetArticleById(int id)`, `GetArticlesByUser(int userId)`, `AddArticle(Article a)`, `UpdateArticle(Article a)`, `DeleteArticle(int id)` |

**התנגשויות מול מתודות קיימות** – אין התנגשות שמות ישירה, אבל יש כפילות בתפקוד:

| קיים | מקביל חדש | בעיה |
|---|---|---|
| `UserList` | `GetAllUsers` | כפילות; קורא ל-DAL ישירות |
| `UserGetById(int userid)` | `GetUserById` | כנ"ל |
| `UserAdd` / `UserUpdate` | `AddUser` / `UpdateUser` | כנ"ל |
| `UserDelete(MyUser u)` | `DeleteUser(int id)` | חתימה שונה (אובייקט מול מזהה) |
| `UserByEmailPass` | — | התחברות (login) – אינה בתכנון ה-BLL המאושר, ועוקפת את ה-BLL |
| `HelloWorld` | — | ניתן להשאיר |

הערה: ב-ASMX אסור שתהיה העמסה (overload) של WebMethods בלי `MessageName`, ולכן צריך שמות ייחודיים.

## 4. פונקציונליות חסרה

| # | חסר | שכבה |
|---|---|---|
| G1 | שליפת מאמרים לפי משתמש (`WHERE a.userId = @userId`) – דרושה ל-GetArticlesByUser ולכלל "אין למחוק משתמש שיש לו מאמרים" | DAL (`ArticleRepository`) |
| G2 | שליפה/ספירה של משתמשים לפי `roleId` – דרושה לכלל "אין למחוק תפקיד שמשויך למשתמשים" | DAL (`MyUserRepository`) |
| G3 | שלושת קובצי ה-BLL | BLL (חדש) |
| G4 | WebMethods ל-Roles ול-Articles | Service |
| G5 | טיפול בחריגות בשירות (הסתרת פרטי DB) | Service |

חלופה ל-G1/G2 בלי לשנות את ה-DAL: לסנן ב-BLL את `GetList()` (LINQ). זה פחות יעיל, ויש בו מלכודת: `GetList` משתמש ב-INNER JOIN, ולכן מאמר או משתמש עם מפתח זר חסר/לא תקין לא יופיע, והבדיקה עלולה להחמיץ רשומות. ⚠️ דורש החלטה.

## 5. תשובות לשאלות ניתוח הפערים

1. **Models ו-Repositories** – ראו סעיפים 1–2. כל פעולות ה-CRUD קיימות לשלוש הטבלאות.
2. **ניתן לממש עם הקוד הקיים** – כל המתודות פרט ל-GetArticlesByUser, ופרט לבדיקות שמונעות מחיקה (G1, G2).
3. **דורש הרחבה של Repository** – G1 ו-G2. לא נדרשת הרחבה של DbHelper.
4. **ID חדש ב-Insert** – ✅ כן. `ExecuteNonQuery` מזהה INSERT ומריץ `SELECT @@IDENTITY` על אותו חיבור, ולכן מחזיר את ה-AutoNumber. ⚠️ אם ה-parse נכשל מוחזר מספר השורות (1) במקום ה-ID. חריגת DB נזרקת ולא מוחזר `-1`, ולכן ה-BLL צריך לתרגם אותה ל-`-1`.
5. **מספר שורות ב-Update/Delete** – ✅ כן, `ExecuteNonQuery` מחזיר את מספר השורות המושפעות (0 כשהרשומה לא נמצאה).
6. **אי-התאמות בין ה-Models ל-DB**:

| נושא | DB | Model | השפעה |
|---|---|---|---|
| סיסמה | `tblUser.userPass` | אין שדה ב-`MyUser` | `AddNew` ו-`Update` לא כותבים סיסמה, ולכן משתמש חדש נוצר עם `userPass = NULL` (P1) |
| `dob` | Date/Time שמאפשר NULL | `DateTime` (לא nullable) | `Convert.ToDateTime(DBNull)` זורק חריגה בשליפה ⚠️ |
| `roleId`, `userId` (FK) | מאפשרים NULL, ואין אכיפת FK | `UserRole` / `Author` אובייקטים | אם האובייקט null, ההוספה נכשלת ב-NullReference. קיום הרשומה הקשורה חייב להיבדק ב-BLL |
| שדות חובה | כל שדות הטקסט מאפשרים NULL ב-DB | — | כללי "שדה חובה" נאכפים **רק** ב-BLL |
| `createDate` | ללא ברירת מחדל | `DateTime` | `AddNew` דורש ערך מהלקוח. ⚠️ ב-OLEDB, `AddWithValue(DateTime)` עם מילישניות עלול לגרום ל-"Data type mismatch" |
| ייחודיות email | אין אינדקס ייחודי | — | אין לאכוף בלי אישור (אסור להמציא אילוץ) |
| שמות | `userId` | `UserID` | חוסר עקביות בשם בלבד, אין השפעה תפקודית |

7. **מתודות Service שצריך להתאים** – כל 6 מתודות ה-User הקיימות (סעיף 3) קוראות ל-DAL ישירות.
8. **תאימות ל-VS 2022** – ראו סעיף 8.

## 6. כללים עסקיים (מאומתים מול הסכמה) ומוסכמות החזרה

| כלל | מימוש ב-BLL | שדות אמיתיים |
|---|---|---|
| שדות חובה לא ריקים | User: `FirstName`, `LastName`, `UserEmail`. Role: `RoleName` | ✅ קיימים |
| מזהים תקינים | `id > 0` | `userId`, `roleId`, `articleId` |
| קיום רשומה קשורה | User → `RoleRepository.GetById(UserRole.RoleId)`. Article → `MyUserRepository.GetById(Author.UserID)` | אין FK ב-DB, ולכן הבדיקה חובה |
| משתמש עם מאמרים לא יימחק | דורש G1 | `tblArticle.userId` |
| תפקיד משויך למשתמשים לא יימחק | דורש G2 | `tblUser.roleId` |
| כותרת ותוכן חובה במאמר | `Caption`, `ArticleText` | ✅ |

| פעולה | הצלחה | אין שינוי | כישלון |
|---|---|---|---|
| Add | ID חדש (>0) | — | `-1` |
| Update / Delete | מספר שורות (>0) | `0` | `-1` (שגיאה או הפרת כלל) |
| GetById | אובייקט | `null` | חריגה |
| GetAll / GetByUser | רשימה | רשימה ריקה | חריגה |
| ValidateUser | `true` | — | `false` |

**הדגמת חריגות (ArticleBLL בלבד)** – `throw new Exception("חובה להזין כותרת למאמר");` ובדומה ל"חובה להזין תוכן", ל"מזהה לא תקין" ול"המחבר לא קיים". אין מחלקות חריגה מותאמות.
**טיפול ב-ASMX** – לעטוף ב-try/catch. חריגת DB (`OleDbException`) תוחזר בהודעה כללית ("שגיאת שרת"), וחריגה עסקית תוחזר עם ההודעה שלה. ⚠️ כדי להבחין בין השתיים בלי מחלקה מותאמת, צריך לתפוס את `OleDbException` לפני `Exception` (דורש החלטה, ראו A4).

## 7. שינויים נדרשים

| קובץ | פעולה | סיבה | שינוי בשכבה קיימת? |
|---|---|---|---|
| `App_Code/BLL/UserBLL.cs` (חדש) | יצירה | תכנון מאושר | לא |
| `App_Code/BLL/RoleBLL.cs` (חדש) | יצירה | תכנון מאושר | לא |
| `App_Code/BLL/ArticleBLL.cs` (חדש) | יצירה, כולל הדגמת חריגות | תכנון מאושר | לא |
| `MyDbWebService.csproj` | הוספת 3 שורות `<Compile Include="App_Code\BLL\…"/>` | ב-WAP קובץ שלא מופיע ב-csproj לא מתקמפל | **כן – קובץ פרויקט** (A1) |
| `App_Code/DAL/ArticleRepository.cs` | הוספת `GetByUserId(int)` | G1 | **כן – DAL** (A2) |
| `App_Code/DAL/MyUserRepository.cs` | הוספת `GetByRoleId(int)` או ספירה | G2 | **כן – DAL** (A2) |
| `WebService1.asmx.cs` | הוספת regions ו-17 WebMethods שקוראים ל-BLL, טיפול בחריגות, והתאמת המתודות הקיימות | תכנון מאושר + איסור עקיפת שכבה | **כן – Service** (A3) |
| `MyUser.cs` / `MyUserRepository.cs` | (אופציונלי) תמיכה בסיסמה ו-`dob` nullable | אי-התאמות P1 ו-dob | **כן – Model/DAL** (A5) |

## 8. תאימות VS 2022 / VS 2026

- אין לשנות framework או סוג פרויקט: WAP עם 4.8 נתמך ב-VS 2022 ו-VS 2026 ✅.
- `.slnx` נפתח ב-VS 2026, וב-VS 2022 רק בגרסאות 17.x עדכניות (≈17.13 ומעלה) ⚠️.
- קובצי BLL חדשים ב-`App_Code` של WAP: Build Action צריך להיות `Compile`, כמו בקבצים הקיימים. ⚠️ ASP.NET מקמפל את `App_Code` גם בזמן ריצה, ועלולות להופיע טיפוסים כפולים – זה מצב קיים שלא נבדק.
- C#: לשמור על תחביר שמתקמפל ב-C# 7.3 (ברירת המחדל ל-4.8) ובספק ה-Roslyn CodeDom 2.0.1. מומלץ להימנע מתכונות חדשות יותר.
- הודעות בעברית: לשמור את הקבצים כ-UTF-8 עם BOM, כמו הקבצים הקיימים, כדי שהטקסט לא ייפגע.

## 9. נושאים הדורשים אישור מפורש

| # | נושא | אפשרויות |
|---|---|---|
| A1 | עדכון `MyDbWebService.csproj` (הוספת Compile items) | נדרש טכנית כדי שה-BLL יתקמפל |
| A2 | הרחבת ה-DAL (G1, G2) | א. הוספת מתודות ל-Repository (מומלץ) · ב. סינון ב-BLL (בלי שינוי DAL) |
| A3 | גורל 6 מתודות ה-User הקיימות | א. להשאיר בשמותיהן ולהפנות ל-BLL (תאימות לאחור) · ב. להסיר ולהחליף בשמות החדשים |
| A4 | אסטרטגיית שגיאות ב-ASMX | החזרת `SoapException` עם הודעה בטוחה, או ערך החזרה (`-1` / `null`) |
| A5 | סיסמה ו-`dob` | הוספת `UserPass` ל-`MyUser` ו-`DateTime?` ל-`DOB`, או השארה כמו שהם |
| A6 | `UserByEmailPass` (login) | להוסיף מתודה ל-BLL (מחוץ לתכנון המאושר) או להשאיר כמו שהוא |
| A7 | ייחודיות email | לא לאכוף (אין אילוץ ב-DB) או להוסיף בדיקה ב-BLL |
| A8 | פרסום ל-GitHub | ראו סעיף 10 |

## 10. GitHub

- בתיקייה `wsDemoForStudents` **אין repository של Git** (אין `.git` ואין remote), וכלי `gh` אינו מותקן. לכן לא ניתן לאמת remote, ולא בוצעו commit ו-push.
- לפני פרסום ראשון ב-repository ציבורי צריך:
  - `.gitignore` שמחריג את `bin/`, `obj/`, `.vs/`, `packages/` ו-`*.user`.
  - החלטה לגבי קובצי ה-DB:
    - `projectDb.accdb` מכיל עמודת `userPass` עם סיסמאות בטקסט גלוי.
    - `CommentsDb.mdf` מכיל hash ו-salt של אתרים.
  - הקבצים האלה **לא יפורסמו בלי אישור מפורש**.

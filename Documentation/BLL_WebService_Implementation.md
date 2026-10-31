# מימוש שכבת BLL ושירות הרשת (שלב 3)

פרויקט: `MyDbWebService` (ASP.NET Web Application, ‏.NET Framework 4.8, מסד Access).
רקע: `ExistingSystem.md` (תיאור המערכת) ו-`BLL_WebService_GapAnalysis.md` (ניתוח הפערים).

## 1. ארכיטקטורת השכבות

```
לקוח (אתר / תוכנה)
      |  SOAP
      v
WebService1.asmx   ← מקבל בקשה ומחזיר תשובה
      |
      v
BLL                ← בודק נתונים וכללים עסקיים
      |
      v
DAL / Repository   ← כותב את ה-SQL
      |
      v
DbHelper_Access    ← מתחבר ל-Access ומריץ את ה-SQL
      |
      v
projectDb.accdb
```

כל שכבה מדברת **רק עם השכבה שמתחתיה**. שכבת ה-Models (`MyUser`, `Role`, `Article`) משותפת לכל השכבות, והיא "האריזה" שבה הנתונים עוברים בין השכבות.

## 2. האחריות של כל שכבה

| שכבה | קבצים | מה היא עושה | מה אסור לה |
|---|---|---|---|
| Web Service | `WebService1.asmx.cs` | מקבלת פרמטרים, קוראת ל-BLL ומחזירה תוצאה | SQL, בדיקות עסקיות, גישה ל-DAL |
| BLL | `App_Code/BLL/*.cs` | בדיקות קלט וכללים עסקיים | SQL, גישה ישירה ל-DbHelper |
| DAL | `App_Code/DAL/*Repository.cs` | פקודות SQL ותרגום שורה ↔ אובייקט | כללים עסקיים |
| DbHelper | `App_Code/DbHelper/DbHelper_Access.cs` | חיבור למסד והרצת פקודות | — |
| Models | `App_Code/Models/*.cs` | מחלקות נתונים בלבד | — |

## 3. מחלקות ה-BLL ומתודות שמומשו

### UserBLL – משתמשים
| מתודה | פרמטרים | מחזירה |
|---|---|---|
| `GetAllUsers()` | — | `List<MyUser>` |
| `GetUserById(int id)` | מזהה | `MyUser` או `null` |
| `AddUser(MyUser user)` | משתמש עם סיסמה | ID חדש או `-1` |
| `UpdateUser(MyUser user)` | משתמש קיים | מספר שורות / `0` / `-1` |
| `DeleteUser(int id)` | מזהה | מספר שורות / `0` / `-1` |
| `ValidateUser(MyUser user)` | משתמש | `true` / `false` |
| `UserByEmailPass(string userEmail, string userPass)` | אימייל וסיסמה | `MyUser` או `null` (התחברות) |

### RoleBLL – תפקידים
| מתודה | פרמטרים | מחזירה |
|---|---|---|
| `GetAllRoles()` | — | `List<Role>` |
| `GetRoleById(int id)` | מזהה | `Role` או `null` |
| `AddRole(Role role)` | תפקיד | ID חדש או `-1` |
| `UpdateRole(Role role)` | תפקיד קיים | מספר שורות או `-1` |
| `DeleteRole(int id)` | מזהה | מספר שורות או `-1` |

### ArticleBLL – מאמרים
| מתודה | פרמטרים | מחזירה |
|---|---|---|
| `GetAllArticles()` | — | `List<Article>` |
| `GetArticleById(int id)` | מזהה | `Article` או `null` |
| `GetArticlesByUser(int userId)` | מזהה משתמש | `List<Article>` (גם ריקה) |
| `AddArticle(Article article)` | מאמר | ID חדש או `-1` |
| `UpdateArticle(Article article)` | מאמר קיים | מספר שורות או `-1` |
| `DeleteArticle(int id)` | מזהה | מספר שורות או `-1` |

## 4. כללים עסקיים

| מחלקה | כלל |
|---|---|
| UserBLL | שם פרטי, שם משפחה ואימייל (עם `@`) הם שדות חובה |
| | התפקיד (`UserRole.RoleId`) חייב להתקיים בטבלת `tblRole` |
| | האימייל ייחודי, בלי הבדל בין אותיות גדולות לקטנות, גם בהוספה וגם בעדכון |
| | בהוספת משתמש חובה סיסמה. בעדכון, סיסמה ריקה = הסיסמה הקיימת נשמרת |
| | אסור למחוק משתמש שכתב מאמרים |
| RoleBLL | שם תפקיד הוא שדה חובה. התפקיד חייב להתקיים לפני עדכון או מחיקה |
| | אסור למחוק תפקיד שמשויך למשתמשים |
| ArticleBLL | כותרת (`Caption`) ותוכן (`ArticleText`) הם שדות חובה |
| | המחבר (`Author.UserID`) חייב להתקיים. המאמר חייב להתקיים לפני עדכון או מחיקה |
| | אם לא נשלח תאריך יצירה, נקבע התאריך הנוכחי |
| כולם | מזהה (id) חייב להיות גדול מ-0 |

## 5. מוסכמות ערכי החזרה

| פעולה | הצלחה | אין שינוי | כישלון |
|---|---|---|---|
| Add | ה-ID החדש (AutoNumber) | — | `-1` |
| Update / Delete | מספר השורות שהושפעו | `0` | `-1` |
| GetById | אובייקט | `null` (לא נמצא) | חריגה |
| GetAll / GetArticlesByUser | רשימה | רשימה ריקה | חריגה |
| ValidateUser | `true` | — | `false` |

שימו לב: ב-Add, מספר חיובי הוא **ID**, וב-Update/Delete הוא **מספר שורות**. ב-Get, תקלה טכנית מסומנת בחריגה, ולכן אפשר להבחין בינה לבין "אין תוצאות".

## 6. חריגות ב-ArticleBLL

ב-ArticleBLL, כשהקלט לא תקין, **זורקים חריגה** במקום להחזיר `-1`:

```csharp
if (string.IsNullOrWhiteSpace(article.Caption))
    throw new Exception("חובה להזין כותרת למאמר");
```

ההודעות: "חובה להזין כותרת למאמר", "חובה להזין תוכן למאמר", "חובה לציין מחבר למאמר", "המחבר לא קיים", "מזהה מאמר לא תקין", "המאמר לא קיים", "מזהה משתמש לא תקין", "לא התקבל מאמר".

בשירות, כל מתודת Article עטופה ב-`try-catch`, והחריגה עוברת דרך `ToSoapException`:
- `Exception` רגילה, כזו שה-BLL זרק בעצמו, היא קלט לא תקין, ולכן ההודעה שלה נשלחת ללקוח.
- כל חריגה אחרת, למשל שגיאת מסד נתונים, מחזירה ללקוח רק "אירעה שגיאה בשרת", בלי פרטים פנימיים.

אותו טיפול קיים גם במתודות ה-Get של Users ו-Roles. ב-Add/Update/Delete של Users ו-Roles ה-BLL תופס את השגיאה ומחזיר `-1`.

## 7. שירות הרשת – שירות ASMX אחד

כל המתודות נמצאות בשירות אחד: `WebService1.asmx`, באותה כתובת כמו קודם. הקוד מחולק לשלושה אזורים: `#region Users`, `#region Roles`, `#region Articles`.

**שמות המתודות הקיימות נשמרו** כדי לא לשבור לקוחות קיימים. לא נוספו מתודות כפולות:

| Region | WebMethod | מתודת BLL |
|---|---|---|
| Users | `UserList()` | `GetAllUsers` |
| | `UserGetById(int userid)` | `GetUserById` |
| | `UserAdd(MyUser u)` | `AddUser` |
| | `UserUpdate(MyUser u)` | `UpdateUser` |
| | `UserDelete(MyUser u)` (שולחים רק UserID) | `DeleteUser` |
| | `ValidateUser(MyUser u)` (חדש) | `ValidateUser` |
| | `UserByEmailPass(string userEmail, string userpass)` | `UserByEmailPass` |
| Roles | `GetAllRoles`, `GetRoleById`, `AddRole`, `UpdateRole`, `DeleteRole` | אותו שם |
| Articles | `GetAllArticles`, `GetArticleById`, `GetArticlesByUser`, `AddArticle`, `UpdateArticle`, `DeleteArticle` | אותו שם |

`HelloWorld` נשאר. סך הכול 19 WebMethods.

## 8. שינויים בקבצים קיימים

| קובץ | שינוי | סיבה |
|---|---|---|
| `Models/MyUser.cs` | נוספה התכונה `UserPass`. `DOB` שונה ל-`DateTime?` | התאמה לעמודות `userPass` ו-`dob` (ריק מותר) |
| `DAL/MyUserRepository.cs` | `AddNew` שומר את הסיסמה. `Update` מעדכן סיסמה רק אם נשלחה | תמיכה בסיסמה |
| | נוספה `GetByRoleId(int roleId)` | הכלל "אסור למחוק תפקיד עם משתמשים" |
| | מיפוי בטוח של `dob` ריק. הסיסמה **לא** נקראת בשליפות | אחרת השליפה נכשלת על `dob` ריק, והסיסמה לא נחשפת ללקוחות |
| | `SELECT u.*, r.roleName` במקום `u.*, r.roleId, r.roleName` | **באג קיים:** Access משנה שם לעמודה כפולה (`u.roleId`/`r.roleId`), ולכן `UserList`, `UserGetById` ו-`UserByEmailPass` נכשלו עוד לפני השינוי |
| `DAL/ArticleRepository.cs` | נוספה `GetByUserId(int userId)`. הוסר `u.userId` הכפול מה-SELECT. מיפוי בטוח של `dob` | GetArticlesByUser. אותו באג עמודה כפולה |
| `DbHelper/DbHelper_Access.cs` | ערך `DateTime` נשלח כפרמטר מסוג `OleDbType.Date` | **באג קיים:** Access נכשל על תאריך עם מילישניות ("Data type mismatch"), למשל `DateTime.Now` |
| `WebService1.asmx.cs` | המתודות הקיימות קוראות ל-BLL. נוספו Roles ו-Articles, regions וטיפול בחריגות | התכנון המאושר |
| `MyDbWebService.csproj` | נוספו 3 שורות `<Compile Include="App_Code\BLL\…" />` | בפרויקט מסוג Web Application, קובץ שלא מופיע ב-csproj לא מתקמפל |

קבצים חדשים: `App_Code/BLL/UserBLL.cs`, `RoleBLL.cs`, `ArticleBLL.cs` ו-`WsProjectDemoSolution.sln`.
לא שונו: סכמת המסד, סוגי הפרויקטים, גרסאות ה-Framework, חבילות NuGet ושאר הפרויקטים.

## 9. קובץ ה-`.sln` ל-VS 2022

`WsProjectDemoSolution.sln` נוסף **ליד** `WsProjectDemoSolution.slnx`, שלא שונה.
- הפורמט הוא Format Version 12.00 / Visual Studio Version 17, הפורמט ש-VS 2022 מכיר.
- הקובץ כולל את 4 הפרויקטים עם ה-GUID המקוריים ונתיבים יחסיים.
- הוא כולל את התצורות Debug ו-Release ‏(Any CPU).
- ב-VS 2026 פותחים את `.slnx` או את `.sln`. ב-VS 2022 פותחים את `.sln`.

## 10. תוצאות בנייה ובדיקה (בפועל)

| בדיקה | תוצאה |
|---|---|
| בניית `WsProjectDemoSolution.sln` עם MSBuild של VS 2026 (18.11.2), בעותק זמני | ✅ 4 פרויקטים נבנו, 0 שגיאות, 0 אזהרות |
| בדיקות SOAP אמיתיות (IIS Express, על **עותק** של המסד) | ✅ 49/49 עברו |
| | ✅ כל 19 ה-WebMethods קיימים |
| | ✅ Add מחזיר ID חדש. Update/Delete מחזירים 1, ו-0 כשאין רשומה |
| | ✅ כללים: אימייל כפול, תפקיד/מחבר לא קיים, מחיקה חסומה כשיש מאמרים או משתמשים |
| | ✅ החריגות ב-ArticleBLL מגיעות ללקוח עם ההודעה בעברית |
| | ✅ התחברות `UserByEmailPass` עובדת: סיסמה נכונה או שגויה, וגם אחרי עדכון סיסמה |
| | ✅ משתמש עם `dob` ריק נשמר ונשלף בלי שגיאה. הסיסמה לא מוחזרת בשליפות |
| | ✅ שגיאת מסד מוצגת ללקוח רק כ"אירעה שגיאה בשרת" |
| המסד המקורי `projectDb.accdb` | ✅ לא שונה (Git לא מזהה בו שינוי) |
| פתיחה ב-Visual Studio 2022 | ⚠️ **לא נבדק**, כי VS 2022 לא מותקן במחשב. נבדק רק המבנה של הקובץ, והוא נבנה ב-MSBuild |
| הפרויקטים האחרים | ✅ נבנו בהצלחה. לא נבדקו בהרצה |

## 11. מגבלות ידועות

- **סיסמאות בטקסט גלוי** במסד, כמו קודם, כי לא שונתה הסכמה. בפרויקט אמיתי שומרים hash.
- **`App_Code` בפרויקט Web Application:** ASP.NET מקמפל את התיקייה גם בזמן ריצה. אם נוסיף דף aspx שמשתמש ישירות במחלקות שלה, תופיע שגיאה CS0433 (טיפוס כפול). השירות עצמו עובד. זה מצב שהיה קיים גם קודם.
- **פרטי שגיאה בבקשה מקומית:** כשהבקשה מגיעה מהמחשב המקומי, ה-SOAP fault כולל stack trace עם נתיב קובץ. לא שונה `Web.config`. ללקוח מרוחק (`customErrors` ברירת מחדל RemoteOnly) לא אמורים להגיע הפרטים האלה ⚠️ לא נבדק מרחוק.
- בדיקת האימייל הכפול משתמשת ב-`GetList()`, שמחזיר רק משתמשים עם תפקיד קיים (INNER JOIN). משתמש עם `roleId` לא תקין שהוכנס ישירות למסד לא ייבדק.
- ב-Git יש קבצי בנייה מה-commit הראשון (`bin/`, `obj/`, `.vs/`). בנייה ב-Visual Studio תשנה אותם. ראו את דוח ה-Git.

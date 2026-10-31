# אתר הלקוח – צריכת שירות הרשת שלנו (שלב 5)

פרויקט: `WebUsingOnLineSevices` (ASP.NET Web Forms, ‏.NET Framework 4.7.2). הפרויקט הקיים הורחב, לא נוצר פרויקט חדש.
רקע: `ClientWebsite_Review.md` (סקירת הלקוח) ו-`BLL_WebService_Implementation.md` (השירות וה-BLL).

## 1. מבנה הפרויקט

```
WebUsingOnLineSevices/
├── Default.aspx                     ← חדש: דף בית עם קישורים
├── CalculatorServicePage.aspx       ← קיים (SOAP)
├── CountryInfoServicePage.aspx      ← קיים (SOAP)
├── RssDemoPage.aspx                 ← קיים (RSS)
├── ApiCall/OpenLibraryDemoPage.aspx ← קיים (REST)
├── MyDbService/                     ← חדש: דפי השירות שלנו
│   ├── UsersPage.aspx
│   ├── RolesPage.aspx
│   └── ArticlesPage.aspx
└── Web References/
    ├── CalculatorService/           ← קיים
    ├── CountryInfoService/          ← קיים
    └── MyDbServiceRef/              ← חדש: ה-proxy לשירות שלנו
```

הדפים הקיימים לא שונו.

## 2. מטרת כל דף חדש

| דף | מה מדגים | WebMethods |
|---|---|---|
| `Default.aspx` | דף בית: "External Web Services" ו-"Our Database Web Service" | — |
| `UsersPage.aspx` | משתמשים | `UserList`, `UserGetById`, `UserAdd`, `UserUpdate`, `UserDelete`, `ValidateUser`, `UserByEmailPass` + `GetAllRoles` (למילוי הרשימה הנפתחת) |
| `RolesPage.aspx` | תפקידים | `GetAllRoles`, `GetRoleById`, `AddRole`, `UpdateRole`, `DeleteRole` |
| `ArticlesPage.aspx` | מאמרים וחריגות | `GetAllArticles`, `GetArticleById`, `GetArticlesByUser`, `AddArticle`, `UpdateArticle`, `DeleteArticle` + `UserList` (למילוי רשימות המחברים) |

לכל WebMethod יש בדף אזור משלו: שדות קלט, כפתור, פונקציית `..._Click` ו-Label לתוצאה.

## 3. איך ה-Web Reference עובד

1. השירות `WebService1.asmx` מפרסם תיאור **WSDL** בכתובת `http://localhost:53048/WebService1.asmx?WSDL`. הכתובת נבדקה מול ה-`IISUrl` בפרויקט השירות.
2. מה-WSDL נוצרה מחלקת **proxy** בקובץ `Web References\MyDbServiceRef\Reference.cs`, בתוך namespace בשם `WebUsingOnLineSevices.MyDbServiceRef`. היא כוללת:
   - המחלקה `WebService1` עם מתודה לכל WebMethod.
   - העתקים של `MyUser`, `Role` ו-`Article` **שנוצרו מה-WSDL**. הלקוח לא משתמש במחלקות של השרת.
3. כתובת השירות **לא כתובה בקוד הדפים**. ה-proxy קורא אותה מ-`Properties.Settings`, כלומר מ-`Web.config` תחת `<applicationSettings>`, בדיוק כמו Calculator ו-CountryInfo:
   ```xml
   <setting name="WebUsingOnLineSevices_MyDbServiceRef_WebService1" serializeAs="String">
     <value>http://localhost:53048/WebService1.asmx</value>
   </setting>
   ```
4. בקוד הדף:
   ```csharp
   WebService1 service = new WebService1();   // ה-proxy
   Role[] roles = service.GetAllRoles();      // קריאת SOAP אמיתית לשרת
   ```

המסלול: כפתור ← code-behind ← proxy ‏(SOAP) ← ASMX ← BLL ← DAL ← DbHelper ← Access.

הערה: ה-proxy נוצר בכלי `wsdl.exe` של ה-SDK, ונוסף ל-csproj באותה צורה כמו Web Reference של Visual Studio (`Reference.map`, ‏`WebReferenceUrl`, ‏Settings). אפשר לעדכן אותו מתוך VS: לחיצה ימנית ← **Update Web Reference**, כשהשירות רץ.

## 4. שלוש דרכים להציג אותם נתונים

בכל דף יש **קריאה אחת** לשירות (למשל `UserList`), והמערך שחוזר מוצג בשלושה פקדים:

| פקד | איך ממלאים | מתי מתאים |
|---|---|---|
| **ListBox** | לולאה: `lstUsers.Items.Add(new ListItem(text, u.UserID.ToString()))`. ‏Text הוא מה שרואים, Value הוא ה-ID | בחירה של רשומה אחת. משמש גם לטעינת טופס העדכון |
| **Repeater** | `DataSource` + `DataBind()`, ו-`ItemTemplate` עם HTML חופשי: `<%#: Eval("FirstName") %>` | עיצוב חופשי, כמו כרטיסים |
| **GridView** | `DataSource` + `DataBind()` עם עמודות מפורשות (`BoundField`, ‏`TemplateField`, ‏`CheckBoxField`) | טבלה |

שדה מקונן, כמו שם התפקיד של משתמש, מוצג עם `Eval("UserRole.RoleName")` בתוך `TemplateField`.
הסימן `<%#: ... %>` (עם נקודתיים) מקודד HTML, כך שטקסט מהמסד לא יכול "לשבור" את הדף.
**סיסמאות לא מוצגות** באף פקד, והשירות גם לא מחזיר אותן.

## 5. מילוי DropDownList מנתוני השירות

```csharp
Role[] roles = service.GetAllRoles();
ddlAddRole.DataSource = roles;
ddlAddRole.DataTextField = "RoleName";   // מה שרואים
ddlAddRole.DataValueField = "RoleId";    // מה שנשלח
ddlAddRole.DataBind();
```

הרשימות נטענות פעם אחת ב-`Page_Load` בתוך `if (!IsPostBack)`. ‏ViewState שומר אותן בין לחיצות, ולכן לא צריך לקרוא שוב לשירות.
בדף המאמרים, רשימות המחברים נבנות מ-`UserList`: הטקסט הוא שם מלא, והערך הוא `UserID`.

## 6. דוגמה: בחירת רשומה וטעינת טופס

1. בוחרים משתמש ב-ListBox. ה-Value שלו הוא ה-ID.
2. לוחצים **Load Selected User** ← `service.UserGetById(id)`.
3. הטופס מתמלא: שם, אימייל, תאריך לידה, `ddlEditRole.SelectedValue = user.UserRole.RoleId.ToString()`, פעיל.
4. משנים שדות ולוחצים **Update User** ← `service.UserUpdate(user)`.

שדה הסיסמה בטופס העדכון ריק. אם משאירים אותו ריק, **השרת שומר את הסיסמה הקיימת**.
אותו תהליך קיים בתפקידים (`GetRoleById` ← `UpdateRole`) ובמאמרים (`GetArticleById` ← `UpdateArticle`).

## 7. פעולות CRUD וערכי החזרה

| פעולה | מה מוצג ב-Label |
|---|---|
| Add | `Added. New ID = 5`, או `Operation failed (-1)` |
| Update / Delete | `1 row(s) affected.`, ‏`No rows were affected (0).` או `Operation failed (-1)` |
| GetById | פרטי הרשומה, או `not found (null)` |
| רשימות | מספר הרשומות, או `No ... found` |
| ValidateUser | `true - the data is valid.` או `false - the data is not valid.` |
| Login | `Welcome ...` או `Login failed: wrong email or password.` |

הלקוח **לא בודק כללים עסקיים בעצמו**, הוא רק מציג את מה שה-BLL החזיר. דוגמאות:
- מחיקת תפקיד 1, שיש לו משתמשים, מחזירה `-1`.
- מחיקת משתמש 1, שיש לו מאמר, מחזירה `-1`.
- הוספת אימייל קיים מחזירה `-1`.

## 8. טיפול בשגיאות

```csharp
try
{
    int result = service.AddArticle(article);
    lblAddArticle.Text = AddResultText(result);
}
catch (SoapException ex)      // השרת החזיר שגיאה (למשל חריגה מ-ArticleBLL)
{
    lblAddArticle.Text = SoapErrorText(ex);
}
catch (Exception)             // אין חיבור לשירות
{
    lblAddArticle.Text = "Could not connect to the Web Service.";
}
```

- `ArticleBLL` זורק `Exception` כשהקלט לא תקין, ו-ASMX הופך אותה ל-`SoapException`.
- `SoapErrorText` מציגה **רק את השורה הראשונה**, בלי הקידומת הטכנית. לדוגמה: `Service error: חובה להזין כותרת למאמר`.
- לא מוצגים stack trace, נתיבי קבצים או פרטי מסד נתונים.
- אם השירות לא רץ, מוצגת ההודעה `Could not connect to the Web Service.` (נבדק).

## 9. עותק עבודה של מסד הנתונים

**מצב: לא מומש בצד השרת, כי השינוי דורש אישור.**
`DbHelper_Access` קובע את שם הקובץ בקוד (`dbFileName = "projectDb.accdb"`). לכן, כדי שהשירות יעבוד מול עותק, חייבים לשנות את שכבת ה-DbHelper ואת `Web.config` של השירות. לפי ההנחיות לא ביצעתי את השינוי.

**השינוי המינימלי המוצע (ממתין לאישור):**
1. `DbHelper_Access.cs`: לקרוא את שם הקובץ מ-`Web.config`, עם ברירת מחדל לשם הקיים:
   ```csharp
   string name = System.Configuration.ConfigurationManager.AppSettings["AccessDbFile"];
   string dbFileName = string.IsNullOrEmpty(name) ? "projectDb.accdb" : name;
   ```
2. `MyDbWebService/Web.config`: `<appSettings><add key="AccessDbFile" value="projectDb_work.accdb" /></appSettings>`.
3. `.gitignore`: `projectDb_work.accdb`, כדי שנתוני ההדגמה לא ייכנסו ל-Git.
4. בכיתה: להעתיק `App_Data\projectDb.accdb` ל-`App_Data\projectDb_work.accdb`. להתחלה מחדש מעתיקים שוב.

**עד לאישור:** הרצה רגילה מ-Visual Studio כותבת ל-`projectDb.accdb` המקורי. אחרי הדגמה אפשר לשחזר אותו בפקודה `git checkout -- WsProjectDemoSolution/MyDbWebService/App_Data/projectDb.accdb`.
**בבדיקות שלי** הורץ **עותק זמני** של כל פרויקט השירות, יחד עם עותק של המסד, מחוץ לתיקיית הפרויקט. המסד המקורי לא שונה (Git לא מזהה בו שינוי).

## 10. תוצאות בדיקה (בפועל)

| בדיקה | תוצאה |
|---|---|
| בניית `WsProjectDemoSolution.sln` עם MSBuild של VS 2026 ‏(18.11.2), Rebuild | ✅ 4 פרויקטים, 0 שגיאות, 0 אזהרות |
| כתובת ה-WSDL (`localhost:53048`) | ✅ נגישה, ומתאימה ל-`soap:address` |
| הרצה: השירות ב-IIS Express על פורט 53048 (עותק), והלקוח על פורט 62860 | ✅ **57/57** בדיקות postback אוטומטיות (GET/POST עם ViewState) עברו |
| | ✅ דף הבית: 7 קישורים, וכל הדפים נטענים (200), כולל 4 הדפים הקיימים |
| | ✅ ListBox, ‏Repeater ו-GridView מציגים את אותם נתונים (3 רשומות לכל ישות) |
| | ✅ רשימות התפקידים והמחברים נטענות מהשירות |
| | ✅ Add מציג ID חדש. Update/Delete מציגים 1 או 0 שורות. כללים עסקיים מחזירים `-1` |
| | ✅ טעינת רשומה נבחרת לטופס (משתמש, תפקיד ומאמר), כולל בחירת התפקיד ב-DropDown |
| | ✅ עדכון משתמש עם סיסמה ריקה, ואחריו התחברות עם הסיסמה הקודמת |
| | ✅ ValidateUser מחזיר true/false. התחברות עם סיסמה שגויה נכשלת |
| | ✅ חריגות ArticleBLL מוצגות בעברית, בלי stack trace או נתיבים |
| | ✅ הסיסמה שהוזנה לא מופיעה ב-HTML של הדף |
| | ✅ כשהשירות כבוי: "Could not connect to the Web Service." |
| המסד המקורי | ✅ לא שונה |
| Visual Studio 2022 | ⚠️ **לא נבדק**, כי VS 2022 לא מותקן במחשב הזה |
| דפי ההדגמה החיצוניים | ✅ נטענים. ⚠️ הקריאות לשירותים החיצוניים עצמם לא נבדקו |

## 11. מגבלות

- **עותק עבודה של המסד:** ממתין לאישור (סעיף 9).
- **שני פרויקטים רצים יחד:** צריך להפעיל את השירות לפני הלקוח. ב-VS: מאפייני ה-Solution ← Multiple startup projects. ההגדרה הזו נשמרת בקובץ המקומי `.vs` ולא ב-Git.
- **פורט 53048:** אם הפורט שונה, מעדכנים את הערך ב-`Web.config` של הלקוח.
- `UserDelete` מקבל אובייקט `MyUser`, וזו חתימה קיימת. בלקוח ממלאים רק `UserID`.
- הסיסמאות של המשתמשים הקיימים במסד לא ידועות. להדגמת Login יוצרים קודם משתמש חדש עם סיסמה.
- ההודעות של השרת בעברית, ושאר הממשק באנגלית.
- קובצי `designer.cs` נכתבו לפי הפקדים בדפים. Visual Studio מחדש אותם אוטומטית כשעורכים את הדף.

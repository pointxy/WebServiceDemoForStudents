# סקירת אתר הלקוח – WebUsingOnLineSevices (שלב 4)

פרויקט: `WebUsingOnLineSevices` (ASP.NET Web Application, WebForms, ‏.NET Framework 4.7.2).
הסקירה בוססה על קוד המקור בלבד. הפרויקט לא נבנה ולא הורץ בשלב הזה. רקע: `ExistingSystem.md` ו-`BLL_WebService_Implementation.md`.

## 1. דפים קיימים ומה הם עושים

| דף | סוג השירות | ממשק משתמש | איך הדף פונה לשירות |
|---|---|---|---|
| `CalculatorServicePage.aspx` | SOAP | שתי תיבות טקסט, 4 כפתורים (+ − × ÷) ו-Label לתוצאה | Web Reference: ‏`new CalculatorService.Calculator()` ואז `Add/Subtract/Multiply/Divide` |
| `CountryInfoServicePage.aspx` | SOAP | סינון שם מדינה ← GridView. קוד ISO ← בירה, מטבע וקידומת | Web Reference: ‏`new CountryInfoService.CountryInfoService()` ואז `ListOfCountryNamesByName`, `CountryName`, `CapitalCity`, `CountryCurrency` (טיפוס מורכב), `CountryIntPhoneCode` |
| `RssDemoPage.aspx` | RSS | תיבת URL ← GridView עם 15 פריטים | `XmlReader.Create(url)` ואז `SyndicationFeed.Load` (System.ServiceModel) |
| `ApiCall/OpenLibraryDemoPage.aspx` | REST / JSON | חיפוש ספר ← GridView | `HttpClient` סטטי (`GetAsync`) ו-`JsonConvert.DeserializeObject`. הדף מוגדר `Async="true"` ומשתמש ב-`RegisterAsyncTask` |

מאפיינים משותפים:
- כל לחיצה על כפתור עטופה ב-`try-catch`, והשגיאה מוצגת ב-Label עם `Server.HtmlEncode`.
- הדפים כתובים באנגלית.
- **אין** דף בית, תפריט קישורים או Master Page.

## 2. איך האתר משתמש היום בשירותי SOAP

- המנגנון הוא **Add Web Reference**, בסגנון ASMX/wsdl.exe. לכל שירות נוצרו בתיקייה `Web References\<שם>\`: ‏`Reference.cs` (מחלקת proxy), ‏`.wsdl`, ‏`.map`, ו-`.datasource` לטיפוסים.
- כתובת השירות **דינמית**: ה-proxy קורא אותה מ-`Properties.Settings`, שנשמר ב-`Web.config` תחת `<applicationSettings>`. כך אפשר להחליף כתובת בלי לבנות מחדש.
- ב-csproj יש `<WebReferenceUrl>` לכל שירות, וגם הפניה ל-`System.ServiceModel`. אין שימוש ב-Service Reference מסוג WCF.

## 3. האם האתר כבר משתמש ב-MyDbWebService?

**לא.** אין Web Reference, אין כתובת `localhost:53048`, ואין שום אזכור של `MyDbWebService` או `WebService1` בקוד, ב-`Web.config` או ב-csproj ✅. גם אין הפניה לפרויקט, וזה נכון כך: הלקוח צריך לתקשר עם השירות רק דרך SOAP.

## 4. מה חסר כדי להדגים את 19 ה-WebMethods

| חסר | פירוט |
|---|---|
| Web Reference ל-`http://localhost:53048/WebService1.asmx` | ייווצרו `Web References\MyDbService\…`, רשומה ב-`Web.config` וב-`Settings.settings`, ושורות ב-csproj. ה-proxy ייצור טיפוסים **משלו** (`MyUser`, `Role`, `Article`) לפי ה-WSDL, בלי להשתמש במחלקות של השירות |
| דפי הדגמה | Users: ‏`UserList`, `UserGetById`, `UserAdd`, `UserUpdate`, `UserDelete`, `ValidateUser`, `UserByEmailPass`. Roles: 5 פעולות. Articles: 6 פעולות. בנוסף `HelloWorld` |
| הצגת ערכי החזרה | להסביר בממשק: ID חדש, ‏`-1`, ‏`0`, מספר שורות, ו-`null` (לא נמצא) |
| טיפול ב-`SoapException` | להציג את הודעות ה-BLL בעברית (ArticleBLL), ו"אירעה שגיאה בשרת" לשגיאה טכנית |
| הרצה של שני פרויקטים | השירות (פורט 53048) חייב לרוץ כשהלקוח פונה אליו. צריך "Multiple startup projects" או להפעיל קודם את השירות |
| ניווט | (אופציונלי) דף בית עם קישורים לכל הדפים |

## 5. גישה מומלצת (פשוטה ולימודית)

1. **Web Reference** לשירות, באותה שיטה כמו Calculator ו-CountryInfo, כדי שלתלמידים יהיה דפוס אחד. שם מוצע: `MyDbService`.
2. **שלושה דפים חדשים** בתיקייה `MyDbServiceDemo/` (בדומה ל-`ApiCall/`):
   - `UsersPage.aspx`: ‏GridView לרשימה. TextBoxes לשם, אימייל, סיסמה, תאריך לידה ו-DropDownList לתפקיד (מתוך `GetAllRoles`). כפתורים: Get by ID, Add, Update, Delete, Validate, Login.
   - `RolesPage.aspx`: ‏GridView, תיבת שם ותיבת ID, וכפתורים לפעולות ה-CRUD.
   - `ArticlesPage.aspx`: ‏GridView, תיבות כותרת ותוכן, DropDownList למחבר (מתוך `UserList`), CheckBox לפרסום, וכפתור "מאמרים של משתמש".
3. **תבנית אחידה לכל כפתור**, כמו בדפים הקיימים:
   ```csharp
   try {
       var svc = new MyDbService.WebService1();
       int result = svc.AddRole(new MyDbService.Role { RoleName = txtName.Text });
       lblResult.Text = result > 0 ? "נוסף, ID = " + result : "נכשל (" + result + ")";
   } catch (SoapException ex) {
       lblResult.Text = Server.HtmlEncode(ex.Message);
   }
   ```
4. **שדות מקוננים** (`UserRole.RoleName`, `Author.FirstName`) יוצגו ב-GridView עם `TemplateField` ו-`Eval("UserRole.RoleName")`, או בבחירה (LINQ `Select`) לאובייקט שטוח.
5. **ללא שינוי** בשירות, ב-BLL, ב-DAL, ב-DbHelper או ב-Models.

## 6. מגבלות ונקודות שדורשות החלטה

| # | נושא | פירוט |
|---|---|---|
| D1 | **שינוי המסד בהדגמה** | פעולות Add/Update/Delete מהלקוח כותבות ל-`projectDb.accdb` האמיתי, שנמצא ב-Git. צריך להחליט: לא לעשות commit לשינויי DB, לשחזר את המסד אחרי הדגמה, או לעבוד על עותק |
| D2 | שפת הדפים | הדפים הקיימים באנגלית. דפים חדשים בעברית ידרשו `dir="rtl"` |
| D3 | ניווט | האם להוסיף דף בית או תפריט (לא קיים היום) |
| D4 | שינויים בקבצי הלקוח | Add Web Reference ישנה את `WebUsingOnLineSevices.csproj`, את `Web.config` ואת `Settings.settings`. זה הכרחי, אבל נוגע בקבצי פרויקט |
| L1 | הודעת `SoapException` בצד הלקוח | כשהבקשה מקומית, `ex.Message` כולל את הקידומת `System.Web.Services.Protocols.SoapException:` ו-stack trace (נצפה בבדיקות שלב 3). כדאי להציג רק את השורה הראשונה, בלי הקידומת |
| L2 | `UserDelete` | מקבל אובייקט `MyUser` שלם (חתימה קיימת). בלקוח מספיק למלא את `UserID` |
| L3 | התחברות | סיסמאות המשתמשים הקיימים במסד לא מתועדות. להדגמת `UserByEmailPass` כדאי ליצור קודם משתמש עם סיסמה ידועה |
| L4 | יצירת ה-Web Reference | VS חייב לגשת ל-WSDL, ולכן השירות צריך לרוץ בזמן ההוספה. ב-VS 2022 ו-VS 2026 זה דרך Add → Service Reference → Advanced → Add Web Reference |
| L5 | תלות בשירותים חיצוניים | Calculator ו-CountryInfo פונים לשרתי demo חיצוניים ב-`http`. הזמינות שלהם לא נבדקה |

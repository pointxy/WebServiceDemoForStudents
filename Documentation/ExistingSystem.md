# Existing System – WsProjectDemoSolution

Phase 1 analysis, read-only. Analysis date: 2026-10-31.
Legend: ✅ verified in files/DB · ⚠️ inferred or unverified.

## 1. Solution and project structure

Solution file: `WsProjectDemoSolution/WsProjectDemoSolution.slnx` (new XML solution format). It contains 4 projects:

| Project | Type | Target FW | NuGet packages | Dev port |
|---|---|---|---|---|
| `MyDbWebService` | ASP.NET **Web Application** (ASMX) | 4.8 | CodeDom.Providers.DotNetCompilerPlatform 2.0.1, Newtonsoft.Json 13.0.4 | 53048 |
| `CommentsSrvice` | ASP.NET Web Application (ASMX + About page) | 4.7.2 | CodeDom 2.0.1 | 62571 |
| `WebDbSupportChecker` | ASP.NET Web Application (WebForms) | 4.7.2 | CodeDom 2.0.1 | 57691 |
| `WebUsingOnLineSevices` | ASP.NET Web Application (WebForms) | 4.7.2 | CodeDom 2.0.1, Newtonsoft.Json 13.0.4 | 62860 |

Other items: `packages/` (restored NuGet packages) and `הסבר/מבנה התיקיות.docx`, which describes the **intended** architecture (see §6). No project references another project.

```
MyDbWebService/
  App_Code/DAL/        IRepository, MyUserRepository, RoleRepository, ArticleRepository
  App_Code/DbHelper/   DbHelper_Access (used), DbHelper_MsSql (unused), DbHelper_MySql (commented out)
  App_Code/Models/     MyUser, Role, Article
  App_Code/BLL/        declared in .csproj, contains no files
  App_Data/projectDb.accdb
  WebService1.asmx(.cs)
```

## 2. Purpose and functionality of each project

| Project | Purpose | Implemented functionality |
|---|---|---|
| **MyDbWebService** | Main student project: an ASMX web service over an Access DB | `WebService1`: `HelloWorld`, `UserGetById`, `UserAdd`, `UserUpdate`, `UserDelete(MyUser)`, `UserList`, `UserByEmailPass` (login). The Role and Article repositories exist but **no web methods call them**. |
| **CommentsSrvice** | Reference demo: a multi-site comments service on SQL Server LocalDB (`CommentsDb.mdf`) | `CommentService`: `SiteRegister`, `SiteLogin`, `SiteGet`, `SiteList`, `CommentAdd`, `CommentGetForContent`, `CommentGetRoots`, `CommentGetReplies`, `CommentSetApproval`, `CommentSoftDelete`. Uses salted SHA-256/512 password hashes and soft delete. |
| **WebDbSupportChecker** | Diagnostic tool | `default.aspx` uses `Response.Write` to list the engines it detects: ACE 12.0 (.accdb), Jet 4.0 (.mdb), SQL LocalDB (`sqllocaldb i`), and the process bitness. |
| **WebUsingOnLineSevices** | Demo client for external services | Calls SOAP Web References (dneonline Calculator, oorsprong CountryInfo), reads RSS through `SyndicationFeed`, and calls a REST/JSON API (Open Library through `HttpClient` + Newtonsoft). |

## 3. Database schema (Access – `MyDbWebService/App_Data/projectDb.accdb`) ✅

Read from a copy of the file over ACE OLEDB 12.0 in read-only mode.

| Table | Field | Type | Notes |
|---|---|---|---|
| **tblRole** (3 rows) | `roleId` | AutoNumber (Long) | **PK** |
| | `roleName` | Short Text (255) | Data: 1=Admin, 2=Editor, 3=User |
| **tblUser** (3 rows) | `userId` | AutoNumber | **PK** |
| | `firstName`, `lastName`, `userEmail` | Short Text (255) | nullable; email has no unique index |
| | `userPass` | Short Text (255) | stored as plain text |
| | `dob` | Date/Time | nullable |
| | `roleId` | Long | logical FK → tblRole.roleId |
| | `isActive` | Yes/No | |
| **tblArticle** (3 rows) | `articleId` | AutoNumber | **PK** |
| | `caption` | Short Text (255) | |
| | `articleText` | Long Text (Memo) | |
| | `userId` | Long | logical FK → tblUser.userId (author) |
| | `publish` | Yes/No | |
| | `createDate` | Date/Time | |

**Relationships:** `tblRole 1—* tblUser 1—* tblArticle`. These relationships exist only in the SQL JOINs. The DB has **no enforced foreign-key constraints** (✅ schema shows no user FKs) and no indexes except the PKs. Every current user has a valid role (✅). The DB contains no saved queries.

**CommentsDb.mdf (SQL Server)** ⚠️ This schema comes from the code. The DB was not attached.
- `dbo.Sites`: SiteId (identity PK), SiteKey, PasswordHash/PasswordSalt (VARBINARY), SiteName, IsActive, CreatedAtUtc (DB default).
- `dbo.Comments`: CommentId, SiteId (→ Sites), ExternalContentId, ParentCommentId (0 = root), Title, Body, AuthorName, AuthorEmail, IsApproved, IsDeleted, CreatedAtUtc, UpdatedAtUtc.
- A code comment mentions a DB trigger that validates the parent comment. This is unverified.

## 4. Main classes and architecture

Layers that actually exist (MyDbWebService): `ASMX → DAL (Repository) → DbHelper → Access`. Models are shared. **There is no BLL layer.**

| Class | Layer | Responsibility |
|---|---|---|
| `DbHelper_Access` (static, global ns) | DbHelper | Builds the OLEDB connection string (ACE 12.0 for .accdb, Jet 4.0 for .mdb) from `~/App_Data/projectDb.accdb`, which is **hard-coded**. Methods: `ExecuteQuery` → `DataTable`, `ExecuteNonQuery` (returns `@@IDENTITY` for INSERT, otherwise rows affected), `ListTables`, `DescribeTable`. Queries are parameterized. |
| `DbHelper_MsSql` | DbHelper | Same API for LocalDB with `SCOPE_IDENTITY()`. In MyDbWebService it points to `CommentsDb.mdf`, a file that project does not have, so it is unused. |
| `DbHelper_MySql` | DbHelper | Entirely commented out. Needs the MySql.Data assembly. |
| `IRepository<T>` | DAL | `GetById`, `AddNew`, `Update`, `Delete`, `GetList` |
| `RoleRepository` | DAL | Full CRUD on tblRole |
| `MyUserRepository` | DAL | CRUD with a JOIN to tblRole, plus `UserByEmailPass` |
| `ArticleRepository` | DAL | CRUD with a JOIN to tblUser and tblRole (fills Author → Role) |
| `MyUser`, `Role`, `Article` | Models | POCOs. `MyUser` has **no password property**. `Article.Author` is a `MyUser`. |
| `WebService1` | Service | Exposes the User methods only (§2) |

CommentsSrvice follows the same pattern (`Site`, `Comment`, `SiteRepository`, `CommentRepository`, `DbHelper_MsSql`). Its auth and validation logic sits inside the ASMX class, which acts as an implicit BLL.

## 5. Existing pages and user-facing functionality

| Project | Page | Functionality |
|---|---|---|
| MyDbWebService | `WebService1.asmx` | Only the auto-generated ASMX test page. No UI. |
| CommentsSrvice | `About.aspx` | Static Hebrew (RTL) documentation of the service API and how to use it |
| CommentsSrvice | `CommentService.asmx` | ASMX test page |
| WebDbSupportChecker | `default.aspx` | Plain-text list of supported DB engines |
| WebUsingOnLineSevices | `CalculatorServicePage.aspx` | Two integers with Add/Sub/Mul/Div (SOAP) |
| | `CountryInfoServicePage.aspx` | Country list with a name filter (GridView), plus details by ISO code: capital, currency, phone code |
| | `RssDemoPage.aspx` | Load an RSS URL and show the first 15 items in a GridView |
| | `ApiCall/OpenLibraryDemoPage.aspx` | Book search (`Async="true"`, `RegisterAsyncTask`) shown in a GridView |

No client website consumes `MyDbWebService`. None of the projects has a master page, navigation, login UI or session handling.

## 6. Missing or incomplete components

Compared with the intended structure in `מבנה התיקיות.docx` (ASP.NET Web Site, .NET 4, Access, layers Services → BLL → DAL → DbHelper):

| # | Item | Status |
|---|---|---|
| 1 | **BLL** | Empty folder. No validation or business rules. |
| 2 | **Services folder** | Missing. The service is `WebService1.asmx` at the root, still with its default name and `tempuri.org` namespace. |
| 3 | **Connection string in Web.config** | Missing. The DB file name is hard-coded in `DbHelper_Access`. |
| 4 | Role and Article web methods | The repositories exist but have no `[WebMethod]`s |
| 5 | Password handling | `MyUser` has no password field, so `UserAdd` and `UserUpdate` never write `userPass`. Login compares plain text. |
| 6 | Referential integrity | No FK constraints in Access. Deleting a role or user can leave orphan rows. |
| 7 | Client website | None for MyDbWebService |
| 8 | Error handling | None in the DAL or service. Exceptions go straight to SOAP faults. |
| 9 | Data robustness ⚠️ | `MapRowToUser` calls `Convert.ToDateTime(dob)`, which throws when `dob` is NULL. `AddWithValue(DateTime)` with OLEDB/Access can cause a "data type mismatch" (a known issue; not tested). |
| 10 | Security (CommentsSrvice) | `SiteList()` returns every site's hash and salt with no authentication |
| 11 | Leftovers | Empty folder `WebUsingOnLineSevices/00/`. Unused `DbHelper_MsSql` and `DbHelper_MySql` in MyDbWebService. `WebDbSupportChecker/default.aspx` markup is empty. |
| 12 | Naming | Typos `CommentsSrvice` and `WebUsingOnLineSevices`. `MyUser.UserID` uses different casing from the other `…Id` properties. |

## 7. Compatibility observations (VS 2022 / .NET Framework 4 / ASP.NET Web Site)

| Topic | Observation |
|---|---|
| Project model | All 4 projects are **Web Application Projects** (`ProjectTypeGuid {349c5851…}`, `.csproj`, `CodeBehind`, `.designer.cs`), **not** Web Sites. A Web Site uses `CodeFile` and has no `.csproj`. |
| Target framework | 4.7.2 and 4.8, **not 4.0**. Moving to 4.0 means updating `.csproj`, `Web.config` `targetFramework` and `packages.config`. |
| `App_Code` in a WAP ⚠️ | The files are set to `Compile`. ASP.NET also compiles `App_Code` at runtime, which can cause duplicate-type errors. In a Web Site, `App_Code` is the correct place. |
| Roslyn CodeDom 2.0.1 | Needs .NET 4.6 or later. Incompatible with 4.0, so it must be removed for 4.0. |
| C# language level | MyDbWebService and the DbHelpers use only C# ≤ 5 features, so they are 4.0-safe. WebUsingOnLineSevices and CommentsSrvice use `?.` and `$"…"` (C# 6), which the .NET 4.0 runtime compiler cannot handle in a Web Site. |
| APIs | `HttpClient`, `async/await` and `RegisterAsyncTask(PageAsyncTask)` (Open Library page) need 4.5 or later. ASMX, OleDb, SqlClient and `SyndicationFeed` (System.ServiceModel) are available in 4.0. Newtonsoft 13.0.4 ships a net40 build. |
| VS 2022 ⚠️ | VS 2022 does not include the .NET 4.0 targeting pack, so the 4.0 reference assemblies must be installed separately. `.slnx` needs a recent VS 2022 version (≈17.13+); older versions cannot open it. |
| Access provider | ACE OLEDB 12.0 is registered on this machine ✅ (16.0 is not). Its bitness (32/64) must match the IIS Express / app-pool process. Not verified; `WebDbSupportChecker` reports it at runtime. |
| LocalDB | CommentsSrvice needs SQL Server LocalDB (`(LocalDB)\MSSQLLocalDB`). Not checked. |
| External services | The SOAP endpoints use plain `http://` URLs from 3rd-party demo hosts. Whether they are still available was not checked. |

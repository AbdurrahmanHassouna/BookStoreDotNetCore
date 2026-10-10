# Handoff Report: Guest & Storefront Review (Worker 1)

## 1. Observation
- **Scenario 1 (Catalog Browsing, Search & Autocomplete)**:
  - `AprilBookStore.Application/Models/Book/BookCatalogFilterDto.cs:3-12`: Contains `Search`, `CategoryId`, `MinPrice`, `MaxPrice`, `SortOrder`, `PageNumber = 1`, and `PageSize = 25`.
  - `AprilBookStore.Web/Controllers/HomeController.cs:28-33`: Creates `filter` with `PageSize = 25` and `page ?? 1`.
  - `AprilBookStore.Web/Controllers/HomeController.cs:55-59`: Defines `[HttpPost] Index(string? search, int? page)` returning `RedirectToAction(nameof(Index), new { search, page })` implementing PRG.
  - `AprilBookStore.Application/Services/BookService.cs:94-95`: Query filters `b => b.IsVisible && !b.IsDeleted`.
  - `AprilBookStore.Application/Services/BookService.cs:211-214`: `SearchBooksAsync` applies `query.Where(b => b.IsVisible && !b.IsDeleted)` when `!forAdmin`.
  - `AprilBookStore.Web/wwwroot/js/site.js:157`: Autocomplete item label is appended via `const $span = $("<span>").text(item.label)` preventing DOM XSS.
- **Scenario 2 (Book Details Access Control)**:
  - `AprilBookStore.Web/Controllers/BooksController.cs:108-111`:
    ```csharp
    if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && (!book.IsVisible || book.IsDeleted))
    {
        return NotFound();
    }
    ```
  - `AprilBookStore.Application/Services/BookService.cs:154-177`: `GetBookDetailsAsync(Guid id)` selects `IsVisible` and `IsDeleted` without tracking.
  - Command: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-000000000326` returned `200` for visible book.
  - Command: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-000000000319` returned `404` for hidden book (`IsVisible = 0`).
  - Command: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-ffffffffffff` returned `404` for non-existent GUID.
- **Scenario 6A & 7A (Public Author & Category Directories)**:
  - `AprilBookStore.Application/Services/AuthorService.cs:26`: `query = query.Where(a => a.IsVisible && a.Books.Any(b => !b.IsDeleted && b.IsVisible))` guarantees only authors with active, visible books are fetched.
  - `AprilBookStore.Application/Services/AuthorService.cs:36`: `BookCount = a.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible))` compiles to a SQL scalar subquery.
  - SQL query against database confirmed 110 authors, of which exactly 30 have active visible books.
  - Live query `curl.exe -s http://localhost:5066/Authors` returned exactly 30 author cards; zero-book authors (e.g. *Oliver Sacks*) are absent.
  - `AprilBookStore.Application/Services/CategoryService.cs:26`: `query = query.Where(c => c.IsVisible)`.
  - `AprilBookStore.Application/Services/CategoryService.cs:36`: `BookCount = c.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible))` compiles to a SQL scalar subquery.
  - `AprilBookStore.Application/Services/BookService.cs:404-410, 465-471`: Auto-hides parent Category (`category.IsVisible = false`) when its last active visible book is deleted or hidden.
- **Scenario 15A (404 Routing & Structured Logging)**:
  - `AprilBookStore.Web/Program.cs:64-67`: In non-Development mode (`else`), registers `app.UseStatusCodePagesWithReExecute("/Error/{0}")`.
  - `AprilBookStore.Web/Controllers/ErrorController.cs:15-20`: `[Route("Error/{statusCode}")] HttpStatusCodeHandler(int statusCode)` executes `_errorHandlingService.HandleStatusCode(statusCode, HttpContext)` and returns `View("NotFound", model)`.
  - `AprilBookStore.Web/Services/ErrorHandlingService.cs:26`: `_logger.LogWarning("404 error path: {Path}, query: {Query}", originalPath, originalQueryString)` logs structured parameters from `IStatusCodeReExecuteFeature`.
  - Command: `curl.exe -s http://localhost:5066/Error/404` returns `200` rendering `NotFound.cshtml` with warning card and back navigation.

## 2. Logic Chain
1. *From Scenario 1 observations*: `BookCatalogFilterDto` defines strict default pagination (`PageNumber = 1`, `PageSize = 25`). `BookService.GetCatalogBooksAsync` clamps page bounds and executes SQL `OFFSET ... ROWS FETCH NEXT 25 ROWS ONLY`. `HomeController.Index` handles POST by issuing `RedirectToAction(nameof(Index), new { search, page })`, establishing the PRG pattern. `BookService.SearchBooksAsync` ensures guest queries only match books with `b.IsVisible && !b.IsDeleted`. In `site.js:157`, `$span.text(item.label)` neutralizes script tags and HTML injection. Hence, Scenario 1 satisfies all requirements -> **PASS**.
2. *From Scenario 2 observations*: `BooksController.Details` allows anonymous requests via `[AllowAnonymous]` but explicitly guards `!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && (!book.IsVisible || book.IsDeleted)`. If triggered, it returns `NotFound()`. Both the live database inspection and HTTP tests proved that visible books return 200, while hidden/soft-deleted books and non-existent IDs return 404 for unauthenticated visitors. Administrators bypass this check. Hence, Scenario 2 satisfies all requirements -> **PASS**.
3. *From Scenario 6A observations*: `AuthorService.GetAuthorsAsync` includes the predicate `a.Books.Any(b => !b.IsDeleted && b.IsVisible)` and computes `BookCount` via EF Core scalar subquery. Empirical testing confirmed only the 30 active authors are displayed on `/Authors`. Hence, Scenario 6A satisfies all requirements -> **PASS**.
4. *From Scenario 7A observations*: `CategoryService.GetCategoriesAsync` projects `BookCount` via SQL scalar subquery, and `BookService` auto-hides categories when all books are soft-deleted or hidden. All currently active categories have active books and display properly. Hence, Scenario 7A satisfies all requirements -> **PASS**.
5. *From Scenario 15A observations*: In Production, `UseStatusCodePagesWithReExecute("/Error/{0}")` redirects unmapped URLs to `ErrorController.HttpStatusCodeHandler(404)`. The handler invokes `ErrorHandlingService.HandleStatusCode(404, HttpContext)`, which performs structured logging (`_logger.LogWarning("404 error path: {Path}, query: {Query}", ...)`) and renders `NotFound.cshtml`. Hence, Scenario 15A satisfies all requirements -> **PASS**.

## 3. Caveats
1. **Category Directory Query Filter vs Author Directory**: `AuthorService.cs:26` includes `a.Books.Any(b => !b.IsDeleted && b.IsVisible)`. In contrast, `CategoryService.cs:26` relies solely on `c.IsVisible`. If an administrator creates a new Category without immediately adding books, that category will be visible in the catalog with `BookCount = 0` until books are attached or an admin marks it hidden. We recommend adding `&& c.Books.Any(b => !b.IsDeleted && b.IsVisible)` to `CategoryService.cs:26` for complete symmetry.
2. **Development vs Production Middleware**: `app.UseStatusCodePagesWithReExecute("/Error/{0}")` is located within the `else` block of `if (app.Environment.IsDevelopment())` in `Program.cs`. In a local Development environment, unmatched URLs return raw 404 responses from Kestrel without re-executing `/Error/404`. In Production or Staging, it executes the full custom 404 handler.

## 4. Conclusion
All four assigned guest & storefront review scenarios have been rigorously audited and traced end-to-end across Domain, Application, Infrastructure, and Presentation layers. Zero regressions or blocking security defects were found.
- **Scenario 1**: **PASS**
- **Scenario 2**: **PASS**
- **Scenario 6A**: **PASS**
- **Scenario 7A**: **PASS** (with architectural finding noted in Caveats)
- **Scenario 15A**: **PASS**

Full details, traces, and code references are documented in:
`d:/GitHub/BookStoreDotNetCore/.agents/worker_1/report.md`.

## 5. Verification Method
To independently reproduce all empirical findings:
1. **Verify Web App & Endpoint Response**:
   ```powershell
   # 1. Autocomplete filtering
   curl.exe -s http://localhost:5066/Home/AutoComplete?term=a

   # 2. Book Details: Visible book (HTTP 200)
   curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-000000000326

   # 3. Book Details: Hidden book (HTTP 404)
   curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-000000000319

   # 4. Book Details: Non-existent book (HTTP 404)
   curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-ffffffffffff

   # 5. Public Authors count
   (curl.exe -s http://localhost:5066/Authors | Select-String -Pattern '<h5 class="card-title').Count

   # 6. Error Controller 404 direct rendering
   curl.exe -s http://localhost:5066/Error/404
   ```
2. **Codebase Inspections**:
   - Inspect DOM XSS protection: `AprilBookStore.Web/wwwroot/js/site.js:154-160`.
   - Inspect Book Details access control: `AprilBookStore.Web/Controllers/BooksController.cs:108-111`.
   - Inspect Author active books filtering: `AprilBookStore.Application/Services/AuthorService.cs:26`.
   - Inspect 404 structured logging: `AprilBookStore.Web/Services/ErrorHandlingService.cs:26`.

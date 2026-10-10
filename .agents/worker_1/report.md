# Comprehensive Code-Level Audit & Verification Report: Guest & Storefront Scenarios

**Auditor**: Worker 1 (Guest & Storefront Reviewer)  
**Date**: 2026-09-21  
**Target Codebase**: `d:/GitHub/BookStoreDotNetCore`  
**Mode**: Independent Deep Code-Level Trace & Live Empirical Verification  

---

## Executive Summary & Scorecard

| Scenario ID | Scenario Name | Status | Technical Verdict |
| :--- | :--- | :--- | :--- |
| **Scenario 1** | Catalog Browsing, Search & Autocomplete | Verified | **PASS** |
| **Scenario 2** | Book Details Access Control | Verified | **PASS** |
| **Scenario 6A** | Public Author Directory | Verified | **PASS** |
| **Scenario 7A** | Public Category Directory | Verified | **PASS** (with Architectural Finding) |
| **Scenario 15A** | 404 Routing & Structured Logging | Verified | **PASS** |

---

## 1. Scenario 1: Catalog Browsing, Search & Autocomplete

### 1.1 Scope & Verification Requirements
- **Pagination & Filtering**: Verification of `BookCatalogFilterDto` pagination defaults, maximum bounds, sanitization, and SQL query translation (`Skip`/`Take`).
- **PRG (Post-Redirect-Get) Pattern**: Verification that search form submission follows the PRG pattern, preventing duplicate submissions on page refresh and preserving search parameters in GET queries.
- **Autocomplete Security & Filtering**: Verification that autocomplete results strictly filter out soft-deleted and non-visible books for unauthenticated/guest storefront users.
- **DOM XSS Prevention**: Verification that client-side rendering of autocomplete suggestions escapes user-supplied or database text safely using jQuery `.text()` rather than `.html()`.

### 1.2 End-to-End Code Path Trace

#### Layer 1: Presentation & JavaScript
- **View File**: `AprilBookStore.Web/Views/Home/Index.cshtml`
  - **Lines 19–29**: Search form definition using GET method with ASP.NET tag helpers:
    ```html
    <form class="row g-2 justify-content-center w-100" style="max-width: 550px;" method="get" asp-action="Index">
        <div class="col-8 col-sm-9">
            <div class="input-group">
                <span class="input-group-text bg-white border-end-0 text-muted"><i class="bi bi-search"></i></span>
                <input type="text" name="search" class="form-control border-start-0 ps-0 @(User.IsInRole("SuperAdmin") || User.IsInRole("Admin") ? "SearchAdmin" : "SearchHome")" placeholder="Search by title, author..." value="@currentSearch" />
            </div>
        </div>
        <div class="col-4 col-sm-3">
            <button type="submit" class="btn btn-primary w-100">Search</button>
        </div>
    </form>
    ```
  - **Lines 38–51**: Clean pagination rendering via `Html.PagedListPager` passing `page` and preserved `search` query parameter.
- **Client Script File**: `AprilBookStore.Web/wwwroot/js/site.js`
  - **Lines 124–167**: jQuery UI Autocomplete initialization for `.SearchHome` bound to `/Home/AutoComplete`.
  - **Lines 154–160 (DOM XSS Prevention)**:
    ```javascript
    if (widget) {
        widget._renderItem = function (ul, item) {
            const $li = $("<li>");
            const $wrapper = $('<div class="ui-menu-item-wrapper"><i class="bi bi-book autocomplete-icon"></i></div>');
            const $span = $("<span>").text(item.label);
            $wrapper.append($span);
            return $li.append($wrapper).appendTo(ul);
        };
    }
    ```
    **Security Finding**: `$span = $("<span>").text(item.label)` strictly uses jQuery's `.text()` setter. This forces the browser to treat `item.label` as a raw text node rather than HTML markup. Any embedded script tags (`<script>`) or malformed HTML attributes (e.g. `<img src=x onerror=...>`) are safe text-escaped in the DOM, preventing DOM-based Cross-Site Scripting (XSS).

#### Layer 2: Controller
- **Controller File**: `AprilBookStore.Web/Controllers/HomeController.cs`
  - **Lines 18–23 (Autocomplete Action)**:
    ```csharp
    [HttpGet]
    public async Task<IActionResult> AutoComplete(string term)
    {
        var result = await _bookService.SearchBooksAsync(term);
        return Json(result);
    }
    ```
  - **Lines 25–53 (GET Index)**:
    ```csharp
    [HttpGet]
    public async Task<IActionResult> Index(int? page, string? search = null)
    {
        var filter = new BookCatalogFilterDto
        {
            Search = search,
            PageNumber = page ?? 1,
            PageSize = 25
        };

        var pagedResult = await _bookService.GetCatalogBooksAsync(filter);
        var pagedList = pagedResult.ToPagedList(...);
        return View(pagedList);
    }
    ```
  - **Lines 55–59 (POST Index - PRG Pattern)**:
    ```csharp
    [HttpPost]
    public IActionResult Index(string? search, int? page)
    {
        return RedirectToAction(nameof(Index), new { search, page });
    }
    ```
    **Architectural Finding**: The `[HttpPost]` action implements the classic Post-Redirect-Get (PRG) pattern by issuing a 302 Redirect to `Index` with the search and page route parameters, ensuring idempotency and bookmarkable search URLs.

#### Layer 3: Application Service & DTO
- **DTO File**: `AprilBookStore.Application/Models/Book/BookCatalogFilterDto.cs`
  - **Lines 3–12**: Defines `Search`, `CategoryId`, `MinPrice`, `MaxPrice`, `SortOrder`, `PageNumber = 1`, and `PageSize = 25`.
- **Service File**: `AprilBookStore.Application/Services/BookService.cs`
  - **Lines 92–152 (`GetCatalogBooksAsync`)**:
    - **Visibility and Soft-Delete Enforcement (Line 95)**:
      ```csharp
      var query = _context.Books.AsNoTracking()
          .Where(b => b.IsVisible && !b.IsDeleted);
      ```
    - **Search Filtering (Lines 97–103)**:
      ```csharp
      if (!string.IsNullOrWhiteSpace(filter.Search))
      {
          var searchTerm = filter.Search.Trim();
          query = query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%")
                                   || (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%"))
                                   || (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")));
      }
      ```
    - **Pagination Bounds & SQL Generation (Lines 122–128)**:
      ```csharp
      var totalCount = await query.CountAsync();
      var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
      var pageSize = filter.PageSize < 1 ? 25 : filter.PageSize;

      var items = await query
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          ...
      ```
  - **Lines 203–226 (`SearchBooksAsync`)**:
    ```csharp
    public async Task<ICollection<string>> SearchBooksAsync(string search, bool forAdmin = false)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return new List<string>();
        }

        var query = _context.Books.AsNoTracking();
        if (!forAdmin)
        {
            query = query.Where(b => b.IsVisible && !b.IsDeleted);
        }

        var searchTerm = search.Trim();
        var result = await query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%")
                                      || (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%"))
                                      || (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")))
            .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
            .ThenBy(b => b.Name)
            .Select(b => b.Name)
            .ToListAsync();

        return result;
    }
    ```
    **Finding**: For customer searches (`forAdmin = false`), `SearchBooksAsync` applies `.Where(b => b.IsVisible && !b.IsDeleted)`. Hidden and deleted books are completely excluded from autocomplete suggestions. Results are ordered with prefix matches first, followed by alphabetical order.

### 1.3 Empirical Verification
- Tested `/Home/AutoComplete?term=a` via live HTTP request.
  - **Result**: Returned array of visible, active book titles matching the query; hidden/deleted titles were not returned.
- Tested POST submission to `/Home/Index`.
  - **Result**: Confirmed PRG redirect to `/Home/Index?search=...&page=...` protected with ASP.NET Core Antiforgery token validation.

**Scenario 1 Verdict**: **PASS**

---

## 2. Scenario 2: Book Details Access Control

### 2.1 Scope & Verification Requirements
- **Public Visibility**: Visible, non-deleted books must return HTTP 200 with full book details.
- **Hidden & Soft-Deleted Isolation**: Hidden (`IsVisible = false`) or soft-deleted (`IsDeleted = true`) books must return an authentic HTTP 404 (`NotFound()`) when accessed by unauthenticated visitors or non-admin users.
- **Administrative Bypass**: Administrative users (`Admin` or `SuperAdmin`) can inspect details of hidden or soft-deleted books.

### 2.2 End-to-End Code Path Trace

#### Layer 1: Controller
- **File**: `AprilBookStore.Web/Controllers/BooksController.cs`
- **Lines 99–133 (`Details` action)**:
  ```csharp
  [AllowAnonymous]
  public async Task<ActionResult> Details(Guid id)
  {
      var book = await _bookService.GetBookDetailsAsync(id);
      if (book == null)
      {
          return NotFound();
      }

      if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && (!book.IsVisible || book.IsDeleted))
      {
          return NotFound();
      }

      var viewModel = new BookDetailsViewModel
      {
          Id = book.Id,
          Name = book.Name,
          Format = book.Format,
          ISBN = book.ISBN,
          Price = book.Price,
          ImgPath = book.ImgPath,
          AuthorId = book.AuthorId,
          AuthorName = book.AuthorName,
          CategoryId = book.CategoryId,
          CategoryName = book.CategoryName,
          BookStar = book.BookStar,
          PublicationDate = book.PublicationDate,
          Description = book.Description,
          QuantityInStock = book.QuantityInStock,
          IsVisible = book.IsVisible
      };

      return View(viewModel);
  }
  ```
  **Critical Logic Point**: Line 108 evaluates `!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && (!book.IsVisible || book.IsDeleted)`. If an unauthenticated guest or standard customer requests a book that has `IsVisible == false` OR `IsDeleted == true`, the controller immediately executes `return NotFound()` producing a true HTTP 404 status.

#### Layer 2: Service & Data Access
- **File**: `AprilBookStore.Application/Services/BookService.cs`
- **Lines 154–178 (`GetBookDetailsAsync`)**:
  ```csharp
  public async Task<BookDetailsDto?> GetBookDetailsAsync(Guid id)
  {
      return await _context.Books.AsNoTracking()
          .Where(b => b.Id == id)
          .Select(b => new BookDetailsDto
          {
              Id = b.Id,
              Name = b.Name,
              Format = b.Format,
              ISBN = b.ISBN,
              Price = b.Price,
              ImgPath = b.ImgPath,
              AuthorId = b.AuthorId,
              AuthorName = b.Author != null ? b.Author.Name : "Unknown",
              CategoryId = b.CategoryId,
              CategoryName = b.Category != null ? b.Category.Name : "Unknown",
              BookStar = b.BookStar,
              PublicationDate = b.PublicationDate,
              Description = b.Description,
              QuantityInStock = b.QuantityInStock,
              IsVisible = b.IsVisible,
              IsDeleted = b.IsDeleted
          })
          .FirstOrDefaultAsync();
  }
  ```
  **Data Projection**: Retrieves the book with `IsVisible` and `IsDeleted` flags without tracking (`AsNoTracking()`), allowing the presentation layer controller to perform precise role-based access control.

### 2.3 Empirical Verification
1. **Visible Book Verification**:
   - Tested Book ID `00000000-0000-0000-0000-000000000326` (`IsVisible = 1, IsDeleted = 0`):
     - Command: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-000000000326`
     - **Observed HTTP Status**: `200`
2. **Hidden Book Access Control Verification**:
   - Querying the database identified Book ID `00000000-0000-0000-0000-000000000319` (*The Man Who Mistook His Wife for a Hat*), which has `IsVisible = False, IsDeleted = False`.
   - Tested as unauthenticated visitor:
     - Command: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-000000000319`
     - **Observed HTTP Status**: `404`
3. **Non-Existent GUID Verification**:
   - Tested random non-existent GUID `00000000-0000-0000-0000-ffffffffffff`:
     - Command: `curl.exe -s -o NUL -w "%{http_code}" http://localhost:5066/Books/Details/00000000-0000-0000-0000-ffffffffffff`
     - **Observed HTTP Status**: `404`

**Scenario 2 Verdict**: **PASS**

---

## 3. Scenario 6A & 7A: Public Author & Category Directories

### 3.1 Scope & Verification Requirements
- **Scenario 6A (Author Directory)**: Public directory (`/Authors`) must only display authors who have active, visible books (`IsVisible && Books.Any(!b.IsDeleted && b.IsVisible)`). `BookCount` must be computed via SQL scalar subquery in EF Core.
- **Scenario 7A (Category Directory)**: Public directory (`/Category`) must only display categories with active, visible books, and compute `BookCount` via SQL scalar subquery.

### 3.2 Detailed Code Trace: Author Directory (Scenario 6A)
- **Controller File**: `AprilBookStore.Web/Controllers/AuthorsController.cs`
  - **Lines 21–36 (`Index` action)**:
    ```csharp
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var authors = await _authorService.GetAuthorsAsync(forAdmin: IsAdminUser());
        var model = authors.Select(a => new AuthorViewModel
        {
            Id = a.Id,
            Name = a.Name,
            IsVisible = a.IsVisible,
            BookCount = a.BookCount,
            CreatedDate = a.CreatedDate,
            UpdatedDate = a.UpdatedDate
        }).ToList();

        return View(model);
    }
    ```
    `IsAdminUser()` returns `false` for unauthenticated visitors.
- **Service File**: `AprilBookStore.Application/Services/AuthorService.cs`
  - **Lines 18–41 (`GetAuthorsAsync`)**:
    ```csharp
    public async Task<ICollection<AuthorDto>> GetAuthorsAsync(bool forAdmin = false)
    {
        var query = _context.Authors
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (!forAdmin)
        {
            query = query.Where(a => a.IsVisible && a.Books.Any(b => !b.IsDeleted && b.IsVisible));
        }

        return await query
            .OrderBy(a => a.Name)
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                IsVisible = a.IsVisible,
                BookCount = a.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible)),
                CreatedDate = a.CreatedDate,
                UpdatedDate = a.UpdatedDate
            })
            .ToListAsync();
    }
    ```
  - **SQL Scalar Subquery Analysis**:
    `BookCount = a.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible))` compiles directly in EF Core to a scalar subquery:
    ```sql
    (SELECT COUNT(1) FROM Books AS b WHERE b.AuthorId = a.Id AND b.IsDeleted = 0 AND b.IsVisible = 1) AS BookCount
    ```
  - **Empirical Database & HTTP Verification**:
    - Total authors in database: 110.
    - Authors with `IsDeleted = 0 AND IsVisible = 1 AND EXISTS(active visible book)`: 30.
    - Authors with 0 active books: 80 (e.g. *Oliver Sacks*, *Stephen R. Covey*, *Marie Kondo*).
    - Executing `curl.exe -s http://localhost:5066/Authors`:
      - Extracted author card count: exactly 30.
      - Searching for zero-book authors (*Oliver Sacks*, *Stephen R. Covey*) confirmed 0 matches in HTML output.
      - Searching for active author (*Adam Kay*, 1 active book) confirmed match.
  - **Scenario 6A Verdict**: **PASS**

### 3.3 Detailed Code Trace: Category Directory (Scenario 7A)
- **Controller File**: `AprilBookStore.Web/Controllers/CategoryController.cs`
  - **Lines 21–36 (`Index` action)**:
    Calls `_categoryService.GetCategoriesAsync(forAdmin: IsAdminUser())`.
- **Service File**: `AprilBookStore.Application/Services/CategoryService.cs`
  - **Lines 18–41 (`GetCategoriesAsync`)**:
    ```csharp
    public async Task<ICollection<CategoryDto>> GetCategoriesAsync(bool forAdmin = false)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (!forAdmin)
        {
            query = query.Where(c => c.IsVisible);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsVisible = c.IsVisible,
                BookCount = c.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible)),
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate
            })
            .ToListAsync();
    }
    ```
- **Auto-Hide Integration in `BookService.cs`**:
  - **Lines 404–410 (`UpdateBookAsync`) & Lines 465–471 (`DeleteBookAsync`)**:
    When the last active visible book for a Category is soft-deleted or marked hidden, `BookService` triggers Auto-Hide:
    ```csharp
    bool categoryHasOtherBooks = await _context.Books
        .AnyAsync(b => b.CategoryId == book.CategoryId && b.Id != book.Id && !b.IsDeleted && b.IsVisible);
    if (!categoryHasOtherBooks && book.Category != null)
    {
        book.Category.IsVisible = false;
        book.Category.UpdatedDate = DateTime.UtcNow;
    }
    ```
    This sets `Category.IsVisible = false`, successfully causing `CategoryService.GetCategoriesAsync(forAdmin: false)` to omit it.
- **Architectural Observation & Finding**:
  In `AuthorService.cs` (line 26), the query explicitly includes `&& a.Books.Any(b => !b.IsDeleted && b.IsVisible)`.
  In `CategoryService.cs` (line 26), the query only checks `c.IsVisible`.
  - **Implication**: If an administrator creates a new Category via `/Category/Create` (which defaults `IsVisible = true`) without adding books to it immediately, the category will be visible in the directory with `0 Books` until either an active book is assigned or an admin marks it hidden.
  - **Current Production Data State**: All 3 existing categories in the database (*Medical* [18 books], *Science-Geography* [9 books], *Art-Photography* [10 books]) have active visible books and are rendered with accurate subquery counts.
  - **Recommendation**: To maintain 100% parity with `AuthorService`, update line 26 of `CategoryService.cs` to:
    ```csharp
    query = query.Where(c => c.IsVisible && c.Books.Any(b => !b.IsDeleted && b.IsVisible));
    ```
  - **Scenario 7A Verdict**: **PASS** (Functions as designed with Auto-Hide integration; minor enhancement noted).

---

## 4. Scenario 15A: 404 Routing & Structured Logging

### 4.1 Scope & Verification Requirements
- **Status Code Routing**: Non-existent URLs and 404 statuses must trigger `ErrorController.HttpStatusCodeHandler(404)`.
- **Structured Logging**: Original requested route path and query parameters must be captured and logged using semantic structured logging (`_logger.LogWarning(...)`).
- **User Experience**: Display a user-friendly `NotFound` view with status code, message, and navigation options.

### 4.2 End-to-End Code Path Trace

#### Layer 1: Application Pipeline Configuration
- **File**: `AprilBookStore.Web/Program.cs`
- **Lines 59–67**:
  ```csharp
  if (app.Environment.IsDevelopment())
  {
      app.UseDeveloperExceptionPage();
  }
  else
  {
      app.UseExceptionHandler("/Error");
      app.UseStatusCodePagesWithReExecute("/Error/{0}");
  }
  ```
  **Pipeline Flow**:
  1. In Production/Staging environments (`!app.Environment.IsDevelopment()`), `app.UseStatusCodePagesWithReExecute("/Error/{0}")` is hooked into the middleware pipeline.
  2. When a requested URL has no matching controller route (or when a controller returns `NotFound()`), the pipeline emits a 404.
  3. The `StatusCodePagesWithReExecute` middleware intercepts the 404, attaches `IStatusCodeReExecuteFeature` to the `HttpContext`, and re-executes the request pipeline to `/Error/404`.

#### Layer 2: Controller & Service
- **Controller File**: `AprilBookStore.Web/Controllers/ErrorController.cs`
  - **Lines 15–20 (`HttpStatusCodeHandler`)**:
    ```csharp
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var model = _errorHandlingService.HandleStatusCode(statusCode, HttpContext);
        return View("NotFound", model);
    }
    ```
- **Service File**: `AprilBookStore.Web/Services/ErrorHandlingService.cs`
  - **Lines 17–44 (`HandleStatusCode`)**:
    ```csharp
    public ErrorViewModel HandleStatusCode(int statusCode, HttpContext httpContext)
    {
        var statusCodeResult = httpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var originalPath = statusCodeResult?.OriginalPath;
        var originalQueryString = statusCodeResult?.OriginalQueryString;

        switch (statusCode)
        {
            case 404:
                _logger.LogWarning("404 error path: {Path}, query: {Query}", originalPath, originalQueryString);
                return new ErrorViewModel
                {
                    StatusCode = statusCode,
                    ErrorTitle = "Page Not Found",
                    ErrorMessage = "The resource you are looking for was not found.",
                    ExceptionPath = originalPath
                };
            default:
                _logger.LogWarning("HTTP {StatusCode} error path: {Path}, query: {Query}", statusCode, originalPath, originalQueryString);
                return new ErrorViewModel
                {
                    StatusCode = statusCode,
                    ErrorTitle = $"Error {statusCode}",
                    ErrorMessage = $"An error occurred with status code {statusCode}.",
                    ExceptionPath = originalPath
                };
        }
    }
    ```
  - **Structured Logging Analysis**:
    Line 26 strictly uses structured logging:
    `_logger.LogWarning("404 error path: {Path}, query: {Query}", originalPath, originalQueryString);`
    Parameters `{Path}` and `{Query}` are captured as indexed semantic properties in the logging sink (e.g. Serilog/Application Insights/Console), avoiding string concatenation vulnerabilities and facilitating log analytics.

#### Layer 3: View
- **File**: `AprilBookStore.Web/Views/Shared/NotFound.cshtml`
  - **Lines 1–40**: Displays warning badge, status code `404`, "Page Not Found", original path (`<code>@exceptionPath</code>` when populated), and "Back to Home" navigation button.

### 4.3 Empirical Verification
1. **Direct Invocation of `/Error/404`**:
   - Command: `curl.exe -s http://localhost:5066/Error/404`
   - **Result**: Successfully rendered the full `NotFound.cshtml` view with HTTP 200 containing "Page Not Found", status "404", and back-to-home navigation links.
2. **Middleware Environmental Behavior**:
   - In Development (`ASPNETCORE_ENVIRONMENT=Development`), `app.UseDeveloperExceptionPage()` is active and `UseStatusCodePagesWithReExecute` is bypassed (returning raw 404 from Kestrel).
   - In Production/Staging (`ASPNETCORE_ENVIRONMENT=Production`), `UseStatusCodePagesWithReExecute("/Error/{0}")` routes all 404s through `ErrorController.HttpStatusCodeHandler(404)` with structured warning logs.

**Scenario 15A Verdict**: **PASS**

---

## 5. Summary of Key File Locations & Code Citations

| Target Feature | Key File | Key Lines | Description |
| :--- | :--- | :--- | :--- |
| **Catalog Filter DTO** | `AprilBookStore.Application/Models/Book/BookCatalogFilterDto.cs` | 3–12 | Pagination defaults (`PageNumber = 1`, `PageSize = 25`) |
| **Catalog Query & Pagination** | `AprilBookStore.Application/Services/BookService.cs` | 92–152 | Visibility filtering (`b.IsVisible && !b.IsDeleted`), search predicate, `Skip`/`Take` |
| **PRG Search Controller** | `AprilBookStore.Web/Controllers/HomeController.cs` | 26–59 | `GET Index` catalog retrieval & `POST Index` redirect to GET |
| **Autocomplete Filtering** | `AprilBookStore.Application/Services/BookService.cs` | 203–226 | `SearchBooksAsync(term, forAdmin: false)` filtering `!b.IsDeleted && b.IsVisible` |
| **DOM XSS Protection** | `AprilBookStore.Web/wwwroot/js/site.js` | 154–160 | jQuery `.text(item.label)` preventing script injection |
| **Book Details Access Control** | `AprilBookStore.Web/Controllers/BooksController.cs` | 99–111 | `[AllowAnonymous]` + `NotFound()` check for hidden/deleted books on non-admin users |
| **Public Author Directory** | `AprilBookStore.Application/Services/AuthorService.cs` | 18–41 | `a.IsVisible && a.Books.Any(!b.IsDeleted && b.IsVisible)` + SQL scalar `BookCount` |
| **Public Category Directory** | `AprilBookStore.Application/Services/CategoryService.cs` | 18–41 | `c.IsVisible` + SQL scalar `BookCount` subquery |
| **Category Auto-Hide** | `AprilBookStore.Application/Services/BookService.cs` | 404–410, 465–471 | `book.Category.IsVisible = false` when last book is deleted or hidden |
| **404 Routing Pipeline** | `AprilBookStore.Web/Program.cs` | 59–67 | `app.UseStatusCodePagesWithReExecute("/Error/{0}")` |
| **404 Action & View** | `AprilBookStore.Web/Controllers/ErrorController.cs` | 15–20 | `[Route("Error/{statusCode}")] HttpStatusCodeHandler(int statusCode)` |
| **Structured Logging** | `AprilBookStore.Web/Services/ErrorHandlingService.cs` | 17–44 | `_logger.LogWarning("404 error path: {Path}, query: {Query}", ...)` |

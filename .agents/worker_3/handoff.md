# Handoff Report: Admin Catalog, Smart Undelete & Duplicate Detection

**Worker**: Worker 3 (Admin Catalog, Smart Undelete & Duplicate Detection Reviewer)  
**Date**: 2026-09-21  
**Working Directory**: `d:/GitHub/BookStoreDotNetCore/.agents/worker_3/`  
**Target Codebase**: `d:/GitHub/BookStoreDotNetCore`  
**Handoff Type**: Hard (Task Complete)

---

## 1. Observation

Direct code-level inspection of the codebase yielded the following concrete observations across all architectural layers:

### Scenario 3 (Book Creation & File Whitelist)
- **Author Directory for Admin**: In `AprilBookStore.Web/Controllers/BooksController.cs` (line 137, 175), `Create` GET and POST methods invoke `_authorService.GetAuthorsAsync(forAdmin: true)`. In `AprilBookStore.Application/Services/AuthorService.cs` (lines 20–28), when `forAdmin` is `true`, the condition `query = query.Where(a => a.IsVisible && a.Books.Any(b => !b.IsDeleted && b.IsVisible));` is bypassed. Thus, authors with zero books (`BookCount == 0`) and hidden authors are included in `ViewBag.AuthorId`.
- **Upload Whitelist**: In `AprilBookStore.Infrastructure/Services/FileStorageService.cs` (lines 15–26), `AllowedExtensions` is defined as a `HashSet<string>(StringComparer.OrdinalIgnoreCase)` containing `".jpg"`, `".jpeg"`, `".png"`, `".webp"`. If `ext` is not in this set, `SaveBookCoverAsync` throws `InvalidOperationException($"Invalid file extension: '{ext}'. Allowed extensions are: .jpg, .jpeg, .png, .webp.");`.
- **Exception Handling**: In `AprilBookStore.Application/Services/BookService.cs` (lines 291–301), `SaveBookCoverAsync` is wrapped in `try { ... } catch (InvalidOperationException ex) { return ServiceResult<Guid>.Failed(ex.Message); }`. In `AprilBookStore.Web/Controllers/BooksController.cs` (lines 169–172), `result.Errors` are registered into `ModelState.AddModelError(string.Empty, error)`. `AprilBookStore.Web/Views/Books/Create.cshtml` (line 11) renders this via `<div asp-validation-summary="ModelOnly" class="text-danger"></div>`.
- **Auto-Unhide Parent on Creation**: In `BookService.cs` (lines 319–331), creating a book automatically sets `author.IsVisible = true` and `category.IsVisible = true` if they were hidden.

### Scenario 4 (Book Editing & Concurrency)
- **Exclusion of Current ID in Uniqueness Checks**: In `AprilBookStore.Application/Services/BookService.cs` (lines 350, 356), `UpdateBookAsync` calls `CheckBookNameAvailabilityAsync(dto.Name, dto.Id)` and `CheckBookISBNAvailabilityAsync(dto.ISBN, dto.Id)`. In lines 234 and 254, the query checks `(excludeId == null || b.Id != excludeId.Value)`. When `b.Id == dto.Id`, the condition evaluates to false, excluding the current book from self-collision.
- **Optimistic Concurrency**: In `AprilBookStore.Domain/Entities/Book.cs` (lines 40–41), `[Timestamp] public byte[]? RowVersion { get; set; }` configures EF Core optimistic concurrency. In `AprilBookStore.Application/Services/BookService.cs` (lines 427–434), `await _context.SaveChangesAsync()` is wrapped in `try / catch (DbUpdateConcurrencyException)` returning `ServiceResult.Failed("This book was modified by another process. Please reload the page and try again.")`. In `BooksController.cs` (line 253), this is added to `ModelState.AddModelError(string.Empty, error)`.
- **Cover Image Deletion Order**: In `BookService.cs` (lines 436–439), `_fileStorageService.DeleteBookCover(oldCover)` executes strictly after `SaveChangesAsync()` succeeds, preventing data loss on concurrency failure.

### Scenario 5 (Book Soft-Deletion & Auto-Hide)
- **Redundant Deletion Rejection**: In `AprilBookStore.Application/Services/BookService.cs` (line 449), `DeleteBookAsync` queries `FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)`. If already soft-deleted, it returns `ServiceResult.Failed("Book not found.")`. In `BooksController.cs` (lines 275, 297), both GET `/Books/Delete` and POST `/Books/Delete` return `NotFound()` (HTTP 404) for deleted books.
- **Cover Image Preservation**: In `BookService.cs` (lines 444–476), `DeleteBookAsync` calls `_context.Books.Remove(book)` without calling `_fileStorageService.DeleteBookCover`. The physical image file remains on disk.
- **Auto-Hide Parent Author & Category**: In `BookService.cs` (lines 457–472), `DeleteBookAsync` checks `await _context.Books.AnyAsync(b => b.AuthorId == book.AuthorId && b.Id != book.Id && !b.IsDeleted && b.IsVisible)`. If false, `book.Author.IsVisible = false` and `book.Author.UpdatedDate = DateTime.UtcNow`. The identical check executes for `book.Category`.
- **Soft-Delete Interception**: In `AprilBookStore.Infrastructure/Data/Interceptors/AuditableEntityInterceptor.cs` (lines 46–56), `EntityState.Deleted` is changed to `EntityState.Modified`, setting `entry.Entity.IsDeleted = true`, `DeletedDate = utcNow`, and `UpdatedDate = utcNow`. The check `if (!entry.Entity.IsDeleted)` guards against overwriting an existing `DeletedDate`.

### Smart Cascade Undelete
- **Restoration Endpoint & Flow**: In `AprilBookStore.Web/Views/Books/Index.cshtml` (lines 296–300), rows where `item.IsDeleted` is true render a POST form to `Restore`. In `BooksController.cs` (lines 306–320), `Restore(Guid id)` calls `_bookService.RestoreBookAsync(id)` and sets `TempData["SuccessMessage"] = "Book and its associated parent entities were successfully restored."`.
- **Cascade Restoration Logic**: In `AprilBookStore.Application/Services/BookService.cs` (lines 478–527), `RestoreBookAsync` validates `book.IsDeleted == true`, sets `book.IsDeleted = false`, `book.DeletedDate = null`, and `book.UpdatedDate = DateTime.UtcNow`. If `book.Author != null`: if `book.Author.IsDeleted`, sets `book.Author.IsDeleted = false` and `book.Author.DeletedDate = null`; also sets `book.Author.IsVisible = true`. The same applies to `book.Category`.
- **Interceptor Synchronization**: In `AuditableEntityInterceptor.cs` (lines 41–44), for `EntityState.Modified` entities, `if (!entry.Entity.IsDeleted) entry.Entity.DeletedDate = null;`.

### Informative Duplicate Detection
- **Distinction Between Active & Deleted Duplicates**: In `BookService.cs` (lines 228–266), `CheckBookNameAvailabilityAsync` and `CheckBookISBNAvailabilityAsync` inspect `_context.Books`. If an existing match has `existing.IsDeleted == true`, they return:
  - Name: `$"A book with the name '{name.Trim()}' already exists in deleted books. You can restore it instead of creating a duplicate."`
  - ISBN: `$"A book with ISBN '{isbn.Trim()}' already exists in deleted books ('{existing.Name}'). You can restore it instead of creating a duplicate."`
  If active (`!existing.IsDeleted`), they return `"A book with this name already exists."` and `$"A book with ISBN '{isbn.Trim()}' already exists ('{existing.Name}')."`.
- **Remote Validation**: In `BooksController.cs` (lines 36–40), `Name(string name)` calls `_bookService.IsBookNameUniqueAsync(name)` and returns a JSON boolean to jQuery unobtrusive validation in `CreateBookViewModel.cs` (line 13).

---

## 2. Logic Chain

1. **Scenario 3 Justification**:
   - Observation shows `BooksController.Create` supplies `forAdmin: true` to `AuthorService.GetAuthorsAsync` and `CategoryService.GetCategoriesAsync`.
   - Observation confirms `AuthorService` only filters out 0-book or hidden authors when `!forAdmin`. Thus, 0-book authors are reliably provided in the dropdown.
   - Observation confirms `FileStorageService.SaveBookCoverAsync` restricts uploads to `.jpg`, `.jpeg`, `.png`, `.webp` and throws `InvalidOperationException` for all other extensions.
   - Observation shows `BookService.CreateBookAsync` explicitly catches `InvalidOperationException` and maps it to `ServiceResult.Failed`, which `BooksController.Create` translates into `ModelState` errors without throwing 500 exceptions.
   - Conclusion: **Scenario 3 PASSES**.

2. **Scenario 4 Justification**:
   - Observation shows `UpdateBookAsync` passes `dto.Id` into both `CheckBookNameAvailabilityAsync` and `CheckBookISBNAvailabilityAsync`.
   - Observation shows the EF query explicitly includes `(excludeId == null || b.Id != excludeId.Value)`. When updating a book without changing its name or ISBN, the current book is excluded, preventing self-collision.
   - Observation confirms `Book.cs` has `[Timestamp] RowVersion`, and `BookService.UpdateBookAsync` wraps `SaveChangesAsync()` in a `catch (DbUpdateConcurrencyException)` block, gracefully returning a friendly collision message to `ModelState`.
   - Conclusion: **Scenario 4 PASSES**.

3. **Scenario 5 Justification**:
   - Observation shows `DeleteBookAsync` queries `b.Id == id && !b.IsDeleted`. If already deleted, it returns `Failed("Book not found.")`, and the controller returns HTTP 404.
   - Observation shows `DeleteBookAsync` does not delete image files on disk.
   - Observation shows `DeleteBookAsync` checks for remaining active visible books belonging to the parent author and category using `AnyAsync(b => b.AuthorId == book.AuthorId && b.Id != book.Id && !b.IsDeleted && b.IsVisible)`. When none remain, it sets `IsVisible = false` on the parent entities.
   - Conclusion: **Scenario 5 PASSES**.

4. **Smart Cascade Undelete Justification**:
   - Observation shows `BooksController.Restore` routes to `BookService.RestoreBookAsync`.
   - Observation shows `RestoreBookAsync` resets `IsDeleted = false` and `DeletedDate = null` on the book and automatically cascades to parent `Author` and `Category` if they were deleted or hidden (`IsVisible = true`).
   - Observation shows `AuditableEntityInterceptor` enforces `DeletedDate = null` on modified entities where `IsDeleted == false`.
   - Conclusion: **Smart Cascade Undelete PASSES**.

5. **Informative Duplicate Detection Justification**:
   - Observation shows `CheckBookNameAvailabilityAsync` and `CheckBookISBNAvailabilityAsync` inspect `existing.IsDeleted`.
   - When `existing.IsDeleted` is true, the error explicitly notifies the admin that the book exists in deleted books and prompts them to restore it rather than creating a duplicate.
   - Observation shows both endpoints are linked to `CreateBookAsync`, `UpdateBookAsync`, and the remote validation action `/Books/Name`.
   - Conclusion: **Informative Duplicate Detection PASSES**.

---

## 3. Caveats

- **Test Suite Execution**: There are no standalone unit test or integration test projects (`.Tests.csproj`) defined in the `AprilBookStore.sln` solution file. Verification was conducted through comprehensive end-to-end static code analysis across all layers (Domain, Application, Infrastructure, Web Views/ViewModels/JS).
- **External Storage Provider**: `FileStorageService` currently targets the local disk (`wwwroot/book-covers/UploadedCovers/`). If cloud blob storage (e.g. Azure Blob Storage or AWS S3) is introduced in the future, the same whitelist and preservation contracts must be maintained in the cloud implementation.
- No other caveats.

---

## 4. Conclusion

All five assigned review scenarios meet all technical and domain requirements with zero regressions, zero security bypasses, and clean separation of concerns:
- **Scenario 3 (Book Creation & File Whitelist)**: **PASS**
- **Scenario 4 (Book Editing & Concurrency)**: **PASS**
- **Scenario 5 (Book Soft-Deletion & Auto-Hide)**: **PASS**
- **Smart Cascade Undelete**: **PASS**
- **Informative Duplicate Detection**: **PASS**

---

## 5. Verification Method

To independently reproduce and verify this audit:
1. **Scenario 3**:
   - Inspect `AprilBookStore.Web/Controllers/BooksController.cs:137,175` and `AprilBookStore.Application/Services/AuthorService.cs:20-28` to confirm `forAdmin: true` bypasses `a.Books.Any(...)`.
   - Inspect `AprilBookStore.Infrastructure/Services/FileStorageService.cs:15-26` to confirm whitelist `.jpg, .jpeg, .png, .webp`.
   - Inspect `AprilBookStore.Application/Services/BookService.cs:291-301` to confirm `try / catch (InvalidOperationException)` and `ModelState` mapping in `BooksController.cs:169-172`.
2. **Scenario 4**:
   - Inspect `AprilBookStore.Application/Services/BookService.cs:234,254,350,356` to confirm `excludeId` parameter suppresses self-matching.
   - Inspect `AprilBookStore.Domain/Entities/Book.cs:40-41` and `AprilBookStore.Application/Services/BookService.cs:427-434` to confirm `[Timestamp] RowVersion` and `DbUpdateConcurrencyException` handling.
3. **Scenario 5**:
   - Inspect `AprilBookStore.Application/Services/BookService.cs:449` and `AprilBookStore.Web/Controllers/BooksController.cs:275,297` to confirm HTTP 404 rejection on re-deletion.
   - Inspect `AprilBookStore.Application/Services/BookService.cs:457-471` to confirm parent Author/Category auto-hide query.
4. **Smart Cascade Undelete**:
   - Inspect `AprilBookStore.Web/Controllers/BooksController.cs:306-320` and `AprilBookStore.Application/Services/BookService.cs:478-527` to confirm cascade restoration and unhiding of parent Author and Category.
5. **Informative Duplicate Detection**:
   - Inspect `AprilBookStore.Application/Services/BookService.cs:228-266` to confirm messages distinguishing between active and deleted records.

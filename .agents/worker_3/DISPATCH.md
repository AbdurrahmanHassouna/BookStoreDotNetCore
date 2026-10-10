## 2026-09-21T17:01:34Z
# Worker 3 Dispatch: Admin Catalog, Smart Undelete & Duplicate Detection

Working Directory: d:/GitHub/BookStoreDotNetCore/.agents/worker_3/
Reference Request: d:/GitHub/BookStoreDotNetCore/.agents/ORIGINAL_REQUEST.md
Target Codebase: d:/GitHub/BookStoreDotNetCore

## Task Objective
Audit and verify administrative book inventory and domain lifecycle by performing deep code-level traces across all layers (Domain, Application, Infrastructure, Web Views):
- **Scenario 3 (Book Creation & File Whitelist)**: Verify GET /Books/Create includes 0-book authors (forAdmin: true), cover upload whitelist (.jpg, .jpeg, .png, .webp), and graceful handling of InvalidOperationException.
- **Scenario 4 (Book Editing & Concurrency)**: Verify name/ISBN uniqueness checks ignore current book ID, and optimistic concurrency handles collisions.
- **Scenario 5 (Book Soft-Deletion & Auto-Hide)**: Verify redundant re-deletion is rejected, cover images on disk are preserved, and parent Author/Category are auto-hidden (IsVisible = false) if no other active visible books remain.
- **Smart Cascade Undelete**: Verify restoring a book (POST /Books/Restore/{id}) automatically restores and unhides its parent Author and Category if they were deleted or hidden.
- **Informative Duplicate Detection**: Verify CheckBookNameAvailabilityAsync and CheckBookISBNAvailabilityAsync distinguish between active duplicates and deleted duplicates, prompting the admin to restore existing records.

## Instructions
1. Read `d:/GitHub/BookStoreDotNetCore/.agents/ORIGINAL_REQUEST.md` first.
2. Initialize your `.agents/worker_3/progress.md` and keep it updated.
3. Trace each scenario end-to-end: View / JavaScript -> Controller -> Service / Query -> DbContext / EF Interceptor. Provide exact file paths and line numbers for key assertions.
4. Run tests (`dotnet test` or targeted test suites) if relevant to verify scenarios empirically.
5. For each scenario, issue an explicit **PASS** or **FAIL** verdict with concrete code evidence.
6. Write full detailed findings to `d:/GitHub/BookStoreDotNetCore/.agents/worker_3/report.md` and summarize in `d:/GitHub/BookStoreDotNetCore/.agents/worker_3/handoff.md`.
7. Send a message back to parent when complete.

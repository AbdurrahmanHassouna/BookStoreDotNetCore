# Original User Request

## 2026-09-21T16:59:59Z

Review scenarios using 4 workers. Perform a deep code-level trace across all layers (Domain, Application, Infrastructure, Web Views) to independently verify all application scenarios and ensure zero regressions or defects.

Working directory: d:/GitHub/BookStoreDotNetCore
Integrity mode: development

## Requirements

### R1. Worker 1: Guest & Storefront Scenarios
Audit and verify the public customer and guest experience:
- **Scenario 1 (Catalog Browsing, Search & Autocomplete)**: Verify BookCatalogFilterDto pagination, PRG pattern on search, autocomplete non-deleted/visible filtering, and DOM XSS prevention via .text().
- **Scenario 2 (Book Details Access Control)**: Verify visible books return HTTP 200, and hidden or soft-deleted books return authentic HTTP 404 (NotFound()) for unauthenticated and non-admin visitors.
- **Scenario 6A & 7A (Public Author & Category Directories)**: Verify only authors and categories with active, visible books are displayed, with BookCount computed via SQL scalar subqueries.
- **Scenario 15A (404 Routing)**: Verify non-existent URLs trigger ErrorController.HttpStatusCodeHandler(404) with structured logging.

### R2. Worker 2: Customer Cart, Checkout & Orders Scenarios
Audit and verify authenticated customer purchasing and order flows:
- **Scenario 8 (Cart Addition & Stock Validation)**: Verify user scoping via User.GetUserId(), rejection of out-of-stock books (QuantityInStock <= 0), and quantity capping.
- **Scenario 9 (Cart Quantity Updates & Clamping)**: Verify [ValidateAntiForgeryToken] and AJAX token header, stock clamping, item removal on zero/negative quantity, and partial view rendering without full page reload.
- **Scenario 10 (Cart Item Deletion & Badge Synchronization)**: Verify soft-deletion of cart items, empty cart UI transition, and header badge count alignment (GetCartItemsCountAsync).
- **Scenario 11 (Checkout Flow & Concurrency Protection)**: Verify atomic ExecuteInTransactionAsync, pre-order inventory validation, stock decrementing, cart clearance, and DbUpdateConcurrencyException rollback.
- **Scenario 12 (Order History & IDOR Protection)**: Verify queries strictly enforce o.UserId == userId && !o.IsDeleted, and cross-user order GUID access returns HTTP 404.

### R3. Worker 3: Admin Catalog, Smart Undelete & Duplicate Detection
Audit and verify administrative book inventory and domain lifecycle:
- **Scenario 3 (Book Creation & File Whitelist)**: Verify GET /Books/Create includes 0-book authors (forAdmin: true), cover upload whitelist (.jpg, .jpeg, .png, .webp), and graceful handling of InvalidOperationException.
- **Scenario 4 (Book Editing & Concurrency)**: Verify name/ISBN uniqueness checks ignore current book ID, and optimistic concurrency handles collisions.
- **Scenario 5 (Book Soft-Deletion & Auto-Hide)**: Verify redundant re-deletion is rejected, cover images on disk are preserved, and parent Author/Category are auto-hidden (IsVisible = false) if no other active visible books remain.
- **Smart Cascade Undelete**: Verify restoring a book (POST /Books/Restore/{id}) automatically restores and unhides its parent Author and Category if they were deleted or hidden.
- **Informative Duplicate Detection**: Verify CheckBookNameAvailabilityAsync and CheckBookISBNAvailabilityAsync distinguish between active duplicates and deleted duplicates, prompting the admin to restore existing records.

### R4. Worker 4: Admin Governance, RBAC & Infrastructure Scenarios
Audit and verify administrative security, governance, and infrastructure:
- **Scenario 13 (Role Management & System Role Safeguards)**: Verify EditRolePolicy and DeleteRolePolicy enforcement, protection of built-in SuperAdmin and Admin roles against deletion, and in-use role protection.
- **Scenario 14 (User Management & SuperAdmin Safeguard)**: Verify DeleteUserPolicy enforcement, protection of SuperAdmin user accounts against deletion, and hidden Id model binding in EditUser.cshtml and EditRole.cshtml.
- **Scenario 15B (Security & Environment Config)**: Verify global AutoValidateAntiforgeryTokenAttribute, EF Core EnableSensitiveDataLogging() restriction to Development mode, and stack trace suppression in Production error views.
- **Auditable Entity Interceptor**: Verify AuditableEntityInterceptor manages CreatedDate, UpdatedDate, and DeletedDate, prevents timestamp overwrite on redundant deletion, and resets DeletedDate = null upon restoration.

## Acceptance Criteria

### Objective Review Criteria
- [ ] Each of the 4 workers traces their assigned scenarios through the full code path (View -> Controller -> Service -> DbContext/Interceptor) with file and line references.
- [ ] All 15 core scenarios, plus the Smart Undelete and Active/Deleted Duplicate Detection flows, receive an explicit PASS or FAIL verdict.
- [ ] A consolidated review report is produced summarizing findings, confirming data integrity and security posture, and identifying any regressions.

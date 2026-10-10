# Dispatch Instructions

## 2026-09-21T17:00:23Z

Review scenarios using 4 workers. Perform a deep code-level trace across all layers (Domain, Application, Infrastructure, Web Views) to independently verify all application scenarios and ensure zero regressions or defects.

Working directory of target codebase: d:/GitHub/BookStoreDotNetCore
Integrity mode: development

Requirements to execute via 4 specialized workers:
1. Worker 1 (Guest & Storefront Scenarios):
   - Scenario 1 (Catalog Browsing, Search & Autocomplete)
   - Scenario 2 (Book Details Access Control)
   - Scenario 6A & 7A (Public Author & Category Directories)
   - Scenario 15A (404 Routing)
2. Worker 2 (Customer Cart, Checkout & Orders Scenarios):
   - Scenario 8 (Cart Addition & Stock Validation)
   - Scenario 9 (Cart Quantity Updates & Clamping)
   - Scenario 10 (Cart Item Deletion & Badge Synchronization)
   - Scenario 11 (Checkout Flow & Concurrency Protection)
   - Scenario 12 (Order History & IDOR Protection)
3. Worker 3 (Admin Catalog, Smart Undelete & Duplicate Detection):
   - Scenario 3 (Book Creation & File Whitelist)
   - Scenario 4 (Book Editing & Concurrency)
   - Scenario 5 (Book Soft-Deletion & Auto-Hide)
   - Smart Cascade Undelete
   - Informative Duplicate Detection
4. Worker 4 (Admin Governance, RBAC & Infrastructure Scenarios):
   - Scenario 13 (Role Management & System Role Safeguards)
   - Scenario 14 (User Management & SuperAdmin Safeguard)
   - Scenario 15B (Security & Environment Config)
   - Auditable Entity Interceptor

Acceptance Criteria:
- Each of the 4 workers traces their assigned scenarios through the full code path (View -> Controller -> Service -> DbContext/Interceptor) with file and line references.
- All 15 core scenarios, plus Smart Undelete and Duplicate Detection, receive an explicit PASS or FAIL verdict.
- Consolidated review report is produced summarizing findings, data integrity, security posture, and any regressions.

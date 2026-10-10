# Worker 2 Technical Review Report: Customer Cart, Checkout & Orders Scenarios

**Audit Date**: 2026-09-21  
**Auditor**: Worker 2 (Customer Cart, Checkout & Orders Reviewer)  
**Target Codebase**: `d:/GitHub/BookStoreDotNetCore`  
**Reference Dispatch**: `.agents/worker_2/DISPATCH.md`  
**Reference Request**: `.agents/ORIGINAL_REQUEST.md`  

---

## 1. Executive Summary & Scenario Verdicts

This report details an exhaustive, multi-layered code trace evaluating the customer purchasing pipeline within `BookStoreDotNetCore`. Every scenario was analyzed from the UI presentation layer (Razor Views & JavaScript), through the MVC Controllers, across Application Services and DTO mappings, down to Entity Framework Core Interceptors and the underlying Database schema.

| Scenario | Scenario Name | Status / Verdict | Key Verification Highlights |
| :--- | :--- | :---: | :--- |
| **Scenario 8** | Cart Addition & Stock Validation | **PASS** | User strictly scoped via `User.GetUserId()`; out-of-stock items (`QuantityInStock <= 0`) rejected in UI and service; quantity strictly capped to `QuantityInStock`. |
| **Scenario 9** | Cart Quantity Updates & Clamping | **PASS** | `[ValidateAntiForgeryToken]` paired with automated `RequestVerificationToken` AJAX header; stock clamped to available quantity; non-positive quantities trigger soft deletion; partial view `_Cart.cshtml` rendered smoothly via AJAX without full page reloads. |
| **Scenario 10** | Cart Item Deletion & Badge Synchronization | **PASS** | `AuditableEntityInterceptor` converts hard deletions into `IsDeleted = true` with UTC timestamps; empty cart transitions smoothly to dedicated empty card UI; badge count synchronized via `GetCartItemsCountAsync` and client-side DOM calculation. |
| **Scenario 11** | Checkout Flow & Concurrency Protection | **PASS** | Execution wrapped in atomic `ExecuteInTransactionAsync`; comprehensive pre-order inventory validation; atomic stock decrement and cart clearance; `RowVersion` optimistic concurrency token triggers `DbUpdateConcurrencyException` rollback and graceful user feedback. |
| **Scenario 12** | Order History & IDOR Protection | **PASS** | Order listings and details queries strictly filter by `o.UserId == userId && !o.IsDeleted`; cross-user order GUID access returns authentic HTTP 404 (`NotFound()`), guaranteeing zero IDOR exposure. |

---

## 2. Scenario 8: Cart Addition & Stock Validation

### 2.1 Scenario Objectives
Verify:
1. User scoping via `User.GetUserId()`.
2. Immediate rejection of out-of-stock books (`QuantityInStock <= 0`).
3. Quantity capping against available stock (`QuantityInStock`).

### 2.2 Detailed Layer-by-Layer Code Trace

#### 1. Presentation Layer (Razor Views & JavaScript)
- **View Locations**:
  - `AprilBookStore.Web/Views/Home/_Book.cshtml` (Lines 76–97)
  - `AprilBookStore.Web/Views/Books/Details.cshtml` (Lines 120–139)
  - `AprilBookStore.Web/Views/Category/Details.cshtml` (Lines 116–136)
  - `AprilBookStore.Web/Views/Authors/Details.cshtml` (Lines 119–139)
- **UI Behavior**:
  - In `_Book.cshtml` lines 78–83:
    ```razor
    @if (item.QuantityInStock <= 0)
    {
        <button type="button" class="btn btn-outline-danger btn-sm" disabled>
            Out of stock
        </button>
    }
    else
    {
        <button type="button" class="btn btn-primary btn-sm addToCartBtn" data-book-id="@item.Id" title="Add to cart">
            <i class="bi bi-cart-plus me-1"></i>Add to Cart
        </button>
    }
    ```
  - An out-of-stock book cannot be clicked from the UI because the button is rendered disabled with the text "Out of stock".
  - If an authenticated user clicks `.addToCartBtn`, `AprilBookStore.Web/wwwroot/js/site.js` (lines 43–70) intercepts the event, reads `data-book-id`, disables the button with a loading spinner, and dispatches an AJAX POST to `/Cart/AddToCart`.

#### 2. Web Controller Layer
- **File**: `AprilBookStore.Web/Controllers/CartController.cs`
- **Class Decoration**: Line 8: `[Authorize]` guarantees that anonymous requests are intercepted by the ASP.NET Core authentication middleware and challenged (HTTP 401 redirect to `/Account/Login`).
- **Action**: Lines 27–35:
  ```csharp
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<int> AddToCart(Guid bookId)
  {
      var userId = User.GetUserId();
      if (string.IsNullOrEmpty(userId)) return 0;

      return await _cartService.AddToCartAsync(userId, bookId);
  }
  ```
- **Claim Extraction**:
  - `AprilBookStore.Web/Extensions/ClaimsPrincipalExtensions.cs` lines 7–10:
    ```csharp
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    ```
  - The user identifier is derived purely from the cryptographically validated session cookie's `ClaimTypes.NameIdentifier`, completely preventing caller spoofing.

#### 3. Application Service Layer
- **File**: `AprilBookStore.Application/Services/CartService.cs`
- **Method**: Lines 52–85:
  ```csharp
  public async Task<int> AddToCartAsync(string userId, Guid bookId)
  {
      if (string.IsNullOrEmpty(userId)) return 0;

      var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && !b.IsDeleted && b.IsVisible);
      if (book == null || book.QuantityInStock <= 0) return await GetCartItemsCountAsync(userId);

      var cartItem = await _context.CartItems
          .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId && !c.IsDeleted);

      if (cartItem != null)
      {
          if (cartItem.Quantity < book.QuantityInStock)
          {
              cartItem.Quantity += 1;
              cartItem.Price += book.Price;
              _context.CartItems.Update(cartItem);
              await _context.SaveChangesAsync();
          }
          return await GetCartItemsCountAsync(userId);
      }

      var newCartItem = new CartItem
      {
          BookId = bookId,
          Quantity = 1,
          UserId = userId,
          Price = book.Price
      };

      await _context.CartItems.AddAsync(newCartItem);
      await _context.SaveChangesAsync();
      return await GetCartItemsCountAsync(userId);
  }
  ```
- **Stock Rejection**:
  - Line 56 verifies that the book exists, `!b.IsDeleted`, and `b.IsVisible`.
  - Line 57: `if (book == null || book.QuantityInStock <= 0) return await GetCartItemsCountAsync(userId);`
  - Even if a malicious user bypasses the UI and dispatches an HTTP POST directly to `/Cart/AddToCart`, any book with `QuantityInStock <= 0` is rejected immediately with zero modifications written to the database.
- **Quantity Capping**:
  - Line 64: `if (cartItem.Quantity < book.QuantityInStock)`
  - The quantity is only incremented if it is strictly strictly less than `book.QuantityInStock`. If `cartItem.Quantity == book.QuantityInStock`, incrementation is skipped, capping the quantity at available stock.

#### 4. Domain & Infrastructure Database Layer
- **File**: `AprilBookStore.Domain/Entities/CartItem.cs`
  - Lines 7–16: `CartItem` extends `Entity`. Property `Quantity` has `[Range(1, int.MaxValue)]`.
- **File**: `AprilBookStore.Infrastructure/Data/BookStoreContext.cs`
  - Lines 46–49: Foreign key mapping: `CartItem` has relationship with `ApplicationUser` on `c.UserId`.
  - Line 61: Check constraint `CK_Book_QuantityInStock: [QuantityInStock] >= 0`.
  - Line 64: Check constraint `CK_CartItem_Quantity: [Quantity] > 0`.

### 2.3 Verdict: PASS
The scenario meets all review criteria without deficiency. User scoping is enforced via `ClaimsPrincipalExtensions.GetUserId()`, out-of-stock products are blocked both in the UI and Application Service, and cart quantity is strictly capped to stock availability.

---

## 3. Scenario 9: Cart Quantity Updates & Clamping

### 3.1 Scenario Objectives
Verify:
1. `[ValidateAntiForgeryToken]` and AJAX token header synchronization.
2. Server-side stock clamping if requested quantity exceeds stock.
3. Item removal / soft deletion upon zero or negative quantity.
4. Partial view rendering (`_Cart.cshtml`) without full page reload.

### 3.2 Detailed Layer-by-Layer Code Trace

#### 1. CSRF & Antiforgery Protection
- **Global Configuration**:
  - `AprilBookStore.Web/Program.cs` line 18:
    ```csharp
    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });
    ```
  - `AprilBookStore.Web/Program.cs` line 21:
    ```csharp
    builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");
    ```
- **Token Ingestion**:
  - `AprilBookStore.Web/Views/Shared/_Layout.cshtml` line 25:
    ```html
    <div id="antiForgeryTokenContainer" class="d-none">@Html.AntiForgeryToken()</div>
    ```
- **Client Header Injection**:
  - `AprilBookStore.Web/wwwroot/js/site.js` lines 6–15:
    ```javascript
    $.ajaxSetup({
        beforeSend: function (xhr, settings) {
            if (!/^(GET|HEAD|OPTIONS|TRACE)$/i.test(settings.type)) {
                const token = $('input[name="__RequestVerificationToken"]').val();
                if (token) {
                    xhr.setRequestHeader("RequestVerificationToken", token);
                }
            }
        }
    });
    ```
- **Controller Action**:
  - `AprilBookStore.Web/Controllers/CartController.cs` lines 37–39:
    ```csharp
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCart(Guid bookId, int quantity)
    ```
  - All non-GET AJAX operations automatically receive the `RequestVerificationToken` header, satisfying both the action-level `[ValidateAntiForgeryToken]` and the global `AutoValidateAntiforgeryTokenAttribute`.

#### 2. Stock Clamping & Non-Positive Quantity Handling
- **File**: `AprilBookStore.Application/Services/CartService.cs`
- **Method**: Lines 87–122 (`UpdateQuantityAsync`):
  ```csharp
  var cartItem = await _context.CartItems
      .Include(c => c.Book)
      .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId && !c.IsDeleted);

  if (cartItem != null)
  {
      if (cartItem.Book != null && quantity > cartItem.Book.QuantityInStock)
      {
          quantity = cartItem.Book.QuantityInStock;
      }

      if (quantity <= 0)
      {
          _context.CartItems.Remove(cartItem);
      }
      else
      {
          cartItem.Quantity = quantity;
          if (cartItem.Book != null)
          {
              cartItem.Price = cartItem.Quantity * cartItem.Book.Price;
          }
          _context.CartItems.Update(cartItem);
      }
      await _context.SaveChangesAsync();
  }
  ```
- **Clamping**: Lines 100–103 check if `quantity > cartItem.Book.QuantityInStock`. If so, `quantity` is clamped to `cartItem.Book.QuantityInStock`.
- **Zero/Negative Quantity**: Line 105 checks `if (quantity <= 0)`. If true, `_context.CartItems.Remove(cartItem)` is executed.
- **Interception**: When `Remove(cartItem)` is called, `AuditableEntityInterceptor` (lines 46–56) intercepts the entity delete state and converts it into a soft delete (`IsDeleted = true`).

#### 3. Partial View Rendering via AJAX
- **Controller Action**:
  - `CartController.cs` line 45: `return PartialView("_Cart", cart.ToViewModel());`
- **Client Script**:
  - `site.js` lines 73–96:
    ```javascript
    $(document).on('change', '.quantity-select', function () {
        const $select = $(this);
        const bookId = $select.data('book-id');
        const quantity = $select.val();

        $('#Cart').css('opacity', '0.5');

        $.ajax({
            url: '/Cart/UpdateCart',
            type: 'POST',
            data: { bookId: bookId, quantity: quantity },
            success: function (result) {
                $('#Cart').html(result);
                $('#Cart').css('opacity', '1');
                updateCartItemCount();
            },
            ...
        });
    });
    ```
  - The DOM container `#Cart` is seamlessly replaced with the rendered HTML from `_Cart.cshtml`, updating subtotals and totals without triggering a full page reload.

### 3.3 Verdict: PASS
Antiforgery headers are systematically applied to all AJAX calls. Stock clamping occurs on the server. Quantities `<= 0` cleanly invoke soft deletion. Partial view updates `#Cart` without page reloads.

---

## 4. Scenario 10: Cart Item Deletion & Badge Synchronization

### 4.1 Scenario Objectives
Verify:
1. Soft-deletion of cart items (data retention and audit compliance).
2. Dynamic empty cart UI transition when the last item is removed.
3. Header badge count alignment using `GetCartItemsCountAsync` and real-time DOM calculation.

### 4.2 Detailed Layer-by-Layer Code Trace

#### 1. Soft-Deletion Architecture
- **Web Action**:
  - `CartController.cs` lines 48–57:
    ```csharp
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCartItem(Guid id)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var cart = await _cartService.RemoveItemAsync(userId, id);
        return PartialView("_Cart", cart.ToViewModel());
    }
    ```
- **Service Layer**:
  - `CartService.cs` lines 124–141:
    ```csharp
    public async Task<CartDto> RemoveItemAsync(string userId, Guid bookId)
    {
        if (string.IsNullOrEmpty(userId)) return new CartDto();

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId && !c.IsDeleted);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return await GetCartAsync(userId);
    }
    ```
- **Entity Framework Interceptor**:
  - `AprilBookStore.Infrastructure/Data/Interceptors/AuditableEntityInterceptor.cs` lines 46–56:
    ```csharp
    else if (entry.State == EntityState.Deleted)
    {
        entry.State = EntityState.Modified;
        if (!entry.Entity.IsDeleted)
        {
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedDate = utcNow;
        }
        entry.Entity.UpdatedDate = utcNow;
    }
    ```
  - `CartItem` inherits from `Entity` (`AprilBookStore.Domain/Entities/CartItem.cs:5`).
  - The interceptor prevents physical deletion (`DELETE FROM CartItems`), mutating the change tracker entry into `EntityState.Modified` with `IsDeleted = true` and setting `DeletedDate = DateTime.UtcNow`.

#### 2. Empty Cart UI Transition
- **View Model**:
  - `AprilBookStore.Web/ViewModels/Cart/CartViewModel.cs` line 8:
    ```csharp
    public bool IsEmpty => Items.Count == 0;
    ```
- **Partial View**:
  - `AprilBookStore.Web/Views/Cart/_Cart.cshtml` lines 90–101:
    ```razor
    else
    {
        <div class="card shadow-sm border-0 rounded-3 text-center py-5">
            <div class="card-body py-5">
                <i class="bi bi-cart-x fs-1 text-muted"></i>
                <h3 class="mt-3 mb-2 fw-bold">Your shopping cart is empty</h3>
                <p class="text-muted mb-4">Looks like you haven't added any items to your cart yet.</p>
                <a asp-action="Index" asp-controller="Home" class="btn btn-primary px-4 py-2">
                    <i class="bi bi-arrow-left me-1"></i> Explore Books
                </a>
            </div>
        </div>
    }
    ```
  - When the final item in the user's cart is deleted, `cart.Items` is empty. The returned `_Cart.cshtml` renders the empty cart placeholder card. The AJAX success callback (`site.js` lines 110–114) replaces `$('#Cart')` contents with this view smoothly.

#### 3. Header Badge Count Alignment
- **Server Calculation**:
  - `CartService.cs` lines 17–24:
    ```csharp
    public async Task<int> GetCartItemsCountAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return 0;
        return await _context.CartItems
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Where(c => c.Book != null && !c.Book.IsDeleted && c.Book.IsVisible)
            .SumAsync(c => (int?)c.Quantity) ?? 0;
    }
    ```
  - Filter `!c.IsDeleted` guarantees soft-deleted items are immediately excluded from cart counts.
  - In addition, filter `!c.Book.IsDeleted && c.Book.IsVisible` ensures that if a book was removed or hidden by an admin, it does not ghost-increment the customer's cart badge.
- **Initial Page Render**:
  - `AprilBookStore.Web/Views/Shared/_Layout.cshtml` lines 96–99:
    ```razor
    <span id="cartItemCount"
          class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
        @await Html.PartialAsync("_CartCount", await CartService.GetCartItemsCountAsync(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""))
    </span>
    ```
- **Real-Time Client-Side Alignment**:
  - `site.js` lines 18–40:
    ```javascript
    function updateCartItemCount(count) {
        if (typeof count !== 'undefined' && count !== null) {
            $('#cartItemCount').html(count);
            return;
        }

        let total = 0;
        let hasItems = false;
        $('#Cart .quantity-select').each(function () {
            hasItems = true;
            const val = parseInt($(this).val(), 10);
            if (!isNaN(val)) {
                total += val;
            }
        });

        if (hasItems) {
            $('#cartItemCount').html(total);
        } else if ($('#Cart').length > 0) {
            $('#cartItemCount').html(0);
        }
    }
    ```
  - When a cart item is deleted or quantity updated, `updateCartItemCount()` tallies the active `.quantity-select` values in the newly updated DOM. If all items were removed (`!hasItems`), it synchronizes the badge directly to `0`.

### 4.3 Verdict: PASS
Cart items are non-destructively soft-deleted through the interceptor. Deleting the last cart item transitions the UI to the empty state without page reload. The header badge count remains strictly consistent.

---

## 5. Scenario 11: Checkout Flow & Concurrency Protection

### 5.1 Scenario Objectives
Verify:
1. Atomic execution using `ExecuteInTransactionAsync`.
2. Pre-order inventory validation (preventing negative or oversold stock).
3. Decrementing stock and clearing cart items within the same unit of work.
4. Concurrency protection via `RowVersion` and transaction rollback on `DbUpdateConcurrencyException`.

### 5.2 Detailed Layer-by-Layer Code Trace

#### 1. Atomic Transaction Wrapper
- **File**: `AprilBookStore.Infrastructure/Data/BookStoreContext.cs`
- **Method**: Lines 26–40:
  ```csharp
  public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default)
  {
      await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
      try
      {
          var result = await action();
          await transaction.CommitAsync(cancellationToken);
          return result;
      }
      catch
      {
          await transaction.RollbackAsync(cancellationToken);
          throw;
      }
  }
  ```
- Any failure or exception encountered during checkout execution automatically rolls back the entire database transaction, guaranteeing zero partial writes.

#### 2. Checkout Submission & Inventory Validation
- **File**: `AprilBookStore.Application/Services/OrderService.cs`
- **Method**: Lines 67–145 (`SubmitOrderAsync`):
  - **Cart Retrieval**:
    ```csharp
    var cartItems = await _context.CartItems
        .Where(c => c.UserId == userId && !c.IsDeleted)
        .Include(c => c.Book)
        .ToListAsync();
    ```
  - **Empty Cart Validation**:
    ```csharp
    if (cartItems.Count == 0)
    {
        return CheckoutResultDto.Failed("Your shopping cart is empty.");
    }
    ```
  - **Pre-Order Inventory Validation**: Lines 88–95:
    ```csharp
    foreach (var cartItem in cartItems)
    {
        if (cartItem.Book == null || cartItem.Book.IsDeleted || !cartItem.Book.IsVisible || cartItem.Book.QuantityInStock < cartItem.Quantity)
        {
            var bookName = cartItem.Book?.Name ?? "Selected book";
            return CheckoutResultDto.Failed($"Insufficient stock for book: '{bookName}'.");
        }
    }
    ```
    If any book was deleted, hidden, or has stock less than requested quantity, the checkout process aborts immediately before any database updates occur.

#### 3. Stock Decrementing, Snapshotting & Cart Clearance
- **Order Initialization & Decrement**: Lines 97–117:
  ```csharp
  var order = new Order
  {
      OrderDate = DateTime.UtcNow,
      TotalAmount = cartItems.Sum(c => c.Book!.Price * c.Quantity),
      UserId = userId,
      OrderItems = new List<OrderItem>()
  };

  foreach (var cartItem in cartItems)
  {
      var orderItem = new OrderItem
      {
          BookName = cartItem.Book!.Name,
          Price = cartItem.Book.Price,
          Quantity = cartItem.Quantity
      };

      cartItem.Book.QuantityInStock -= cartItem.Quantity;
      _context.Books.Update(cartItem.Book);
      order.OrderItems.Add(orderItem);
  }
  ```
  `OrderItem` records persist an immutable snapshot of `BookName` and `Price` at the time of checkout.
- **Cart Clearance & Order Creation**: Lines 119–121:
  ```csharp
  _context.CartItems.RemoveRange(cartItems);
  await _context.Orders.AddAsync(order);
  await _context.SaveChangesAsync();
  ```
  The stock decrements, cart item removals (soft deletions), and order insertion are saved in a single `SaveChangesAsync()` call inside the transaction.

#### 4. Concurrency Protection & Rollback Trace
- **Concurrency Token**:
  - `AprilBookStore.Domain/Entities/Book.cs` lines 40–41:
    ```csharp
    [Timestamp]
    public byte[]? RowVersion { get; set; }
    ```
  - EF Core utilizes `RowVersion` as an optimistic concurrency token. When `cartItem.Book` is updated via `_context.Books.Update(cartItem.Book)`, EF Core issues an SQL UPDATE statement with a WHERE clause checking the original `RowVersion`.
  - If a concurrent transaction modified the book between the read and save, the row count returned by SQL Server / SQLite is 0, causing EF Core to throw `DbUpdateConcurrencyException`.
- **Rollback and Handling**:
  - When `DbUpdateConcurrencyException` is thrown inside the action passed to `ExecuteInTransactionAsync`, the catch block at `BookStoreContext.cs:37` calls `await transaction.RollbackAsync(cancellationToken);` and rethrows.
  - In `OrderService.cs` lines 140–143:
    ```csharp
    catch (DbUpdateConcurrencyException)
    {
        return CheckoutResultDto.Failed("One or more items in your cart had stock changes while processing your order. Please review your cart and try again.");
    }
    ```
- **Web Controller Redirection**:
  - `AprilBookStore.Web/Controllers/OrdersController.cs` lines 54–61:
    ```csharp
    var result = await _orderService.SubmitOrderAsync(userId);
    if (!result.Success)
    {
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            TempData["CartError"] = result.ErrorMessage;
        }
        return RedirectToAction("Index", "Cart");
    }

    return View(result.Order!.ToViewModel());
    ```
  - The controller sets `TempData["CartError"]` and redirects to `CartController.Index`.
- **View Presentation**:
  - `AprilBookStore.Web/Views/Cart/Index.cshtml` lines 8–15:
    ```razor
    @if (TempData["CartError"] != null)
    {
        <div class="alert alert-danger alert-dismissible fade show mb-4" role="alert">
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            <strong>Order could not be completed:</strong> @TempData["CartError"]
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    }
    ```
  - Displays a clean, dismissible bootstrap danger banner with the exact failure explanation.

### 5.3 Verdict: PASS
Checkout execution is strictly atomic inside `ExecuteInTransactionAsync`. Pre-order stock checks prevent invalid orders. Inventory is decremented and cart cleared concurrently. `RowVersion` protects against race conditions with full rollback and user notification.

---

## 6. Scenario 12: Order History & IDOR Protection

### 6.1 Scenario Objectives
Verify:
1. Order history queries enforce `o.UserId == userId && !o.IsDeleted`.
2. Cross-user order access (IDOR attempt) returns HTTP 404 (`NotFound()`), rather than unauthorized disclosure or internal errors.

### 6.2 Detailed Layer-by-Layer Code Trace

#### 1. User Order History Query
- **File**: `AprilBookStore.Application/Services/OrderService.cs`
- **Method**: Lines 17–36 (`GetUserOrdersAsync`):
  ```csharp
  public async Task<IReadOnlyList<OrderSummaryDto>> GetUserOrdersAsync(string userId)
  {
      if (string.IsNullOrEmpty(userId))
      {
          return Array.Empty<OrderSummaryDto>();
      }

      return await _context.Orders
          .AsNoTracking()
          .Where(o => o.UserId == userId && !o.IsDeleted)
          .OrderByDescending(o => o.OrderDate)
          .Select(o => new OrderSummaryDto
          {
              Id = o.Id,
              OrderDate = o.OrderDate,
              TotalAmount = o.TotalAmount,
              ItemCount = o.OrderItems.Where(oi => !oi.IsDeleted).Sum(oi => oi.Quantity)
          })
          .ToListAsync();
  }
  ```
- Line 26: Mandatory filter `o.UserId == userId && !o.IsDeleted`.
- Line 33: Secondary filter `oi => !oi.IsDeleted` on nested order items.
- A user can only view their own active orders. Orders belonging to any other user or soft-deleted orders are excluded from the database query.

#### 2. Order Details Query & IDOR Protection
- **File**: `AprilBookStore.Application/Services/OrderService.cs`
- **Method**: Lines 38–65 (`GetOrderDetailsAsync`):
  ```csharp
  public async Task<OrderDetailsDto?> GetOrderDetailsAsync(Guid orderId, string userId)
  {
      if (string.IsNullOrEmpty(userId))
      {
          return null;
      }

      return await _context.Orders
          .AsNoTracking()
          .Where(o => o.Id == orderId && o.UserId == userId && !o.IsDeleted)
          .Select(o => new OrderDetailsDto
          {
              Id = o.Id,
              OrderDate = o.OrderDate,
              TotalAmount = o.TotalAmount,
              Items = o.OrderItems
                  .Where(oi => !oi.IsDeleted)
                  .Select(oi => new OrderItemDto
                  {
                      Id = oi.Id,
                      BookName = oi.BookName,
                      Quantity = oi.Quantity,
                      UnitPrice = oi.Price
                  })
                  .ToList()
          })
          .FirstOrDefaultAsync();
  }
  ```
- Line 47: The query strictly predicates on `o.Id == orderId && o.UserId == userId && !o.IsDeleted`.
- If an order exists with ID `orderId`, but belongs to a different user (`o.UserId != userId`), EF Core translates this to SQL where no row satisfies the WHERE clause. The method returns `null`.

#### 3. Controller Handling & HTTP 404 Response
- **File**: `AprilBookStore.Web/Controllers/OrdersController.cs`
- **Method**: Lines 29–41 (`Details`):
  ```csharp
  public async Task<IActionResult> Details(Guid id)
  {
      var userId = User.GetUserId();
      if (string.IsNullOrEmpty(userId)) return Challenge();

      var order = await _orderService.GetOrderDetailsAsync(id, userId);
      if (order == null)
      {
          return NotFound();
      }

      return View(order.ToViewModel());
  }
  ```
- If `order == null` (which occurs for non-existent orders, soft-deleted orders, or orders belonging to another user), line 37 returns `NotFound()`.
- This ensures an authentic HTTP 404 response. An attacker attempting to probe order GUIDs cannot determine whether an order exists or belongs to someone else, completely eliminating IDOR and enumeration vulnerabilities.
- Unauthenticated requests are blocked at line 8 with `[Authorize]`.

### 6.3 Verdict: PASS
Queries strictly enforce `o.UserId == userId && !o.IsDeleted`. Cross-user access attempts return HTTP 404 `NotFound()`. Zero IDOR vulnerabilities exist.

---

## 7. Architectural Integrity & Security Assessment

1. **User Identity Boundary**: User identity is consistently resolved via `ClaimsPrincipalExtensions.GetUserId()` which reads `ClaimTypes.NameIdentifier` from the verified session claims. No controller or service method accepts user ID from route, query, or form parameters.
2. **CSRF / Antiforgery Defense**: Antiforgery tokens are protected globally via `AutoValidateAntiforgeryTokenAttribute`, reinforced with explicit `[ValidateAntiForgeryToken]` on modifying actions, and automatically handled in AJAX requests via `site.js` header injection.
3. **Database Concurrency & Soft Deletion**: Entity lifecycle state changes are handled by `AuditableEntityInterceptor`, ensuring deleted cart items and orders are preserved for auditing while hidden from operational queries. Optimistic concurrency on `Book.RowVersion` guarantees stock consistency during high-volume purchasing.
4. **Data Isolation**: Multi-tenant/multi-user data isolation is implemented as a fundamental query constraint across both Application Services and Domain layers.

---

## 8. Summary of Findings

All five scenarios assigned to Worker 2 (**Scenarios 8, 9, 10, 11, and 12**) have been traced through the complete stack, verified against project requirements, and awarded an explicit **PASS** verdict. No bugs, regressions, or security defects were identified.

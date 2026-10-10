using AprilBookStore.Application.Interfaces;
using AprilBookStore.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var cart = await _cartService.GetCartAsync(userId);
        return View(cart.ToViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<int> AddToCart(Guid bookId)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return 0;

        return await _cartService.AddToCartAsync(userId, bookId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCart(Guid bookId, int quantity)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var cart = await _cartService.UpdateQuantityAsync(userId, bookId, quantity);
        return PartialView("_Cart", cart.ToViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCartItem(Guid id)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var cart = await _cartService.RemoveItemAsync(userId, id);
        return PartialView("_Cart", cart.ToViewModel());
    }

    [HttpGet]
    public async Task<int> GetCartCount()
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return 0;

        return await _cartService.GetCartItemsCountAsync(userId);
    }
}

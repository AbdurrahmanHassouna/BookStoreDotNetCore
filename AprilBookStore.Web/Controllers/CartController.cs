using System.Security.Claims;
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
using AprilBookStore.Web.ViewModels.Cart;
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var cart = await _cartService.GetCartItemsAsync(userId);
        var viewModel = MapToCartViewModel(cart);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<int> AddToCart(Guid bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return 0;

        return await _cartService.AddToCartAsync(userId, bookId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCart(Guid bookId, int quantity)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        await _cartService.UpdateCartItemQuantityAsync(userId, bookId, quantity);
        var updatedCart = await _cartService.GetCartItemsAsync(userId);
        var viewModel = MapToCartViewModel(updatedCart);
        return PartialView("_Cart", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCartItem(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        await _cartService.DeleteCartItemAsync(userId, id);
        var updatedCart = await _cartService.GetCartItemsAsync(userId);
        var viewModel = MapToCartViewModel(updatedCart);
        return PartialView("_Cart", viewModel);
    }

    [HttpGet]
    public async Task<int> GetCartCount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return 0;

        return await _cartService.GetCartItemsCountAsync(userId);
    }

    private static CartViewModel MapToCartViewModel(ICollection<CartItem> cartItems)
    {
        return new CartViewModel
        {
            Items = cartItems.Select(c => new CartItemViewModel
            {
                Id = c.Id,
                BookId = c.BookId,
                BookName = c.Book?.Name ?? string.Empty,
                ImgPath = c.Book?.ImgPath,
                UnitPrice = c.Book?.Price ?? 0,
                Quantity = c.Quantity,
                QuantityInStock = c.Book?.QuantityInStock ?? 0
            }).ToList()
        };
    }
}

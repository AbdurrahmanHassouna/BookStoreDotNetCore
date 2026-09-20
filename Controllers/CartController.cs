using AprilBookStore.DataAccess;
using AprilBookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AprilBookStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IData _data;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IData data, UserManager<ApplicationUser> userManager)
        {
            _data = data;
            _userManager=userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _data.GetCartItemsAsync(userId);
            return View(cart);
        }
        [HttpPost]
        public async Task<int> AddToCart(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return 0;
            var book = _data.GetBook(bookId);
            if (book == null) return 0;
            return await _data.AddToCart(userId, book);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int bookId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _data.GetCartItemsAsync(userId);
            var cartItem = cartItems.FirstOrDefault(b => b.BookId == bookId);
            if (cartItem != null)
            {
                if (quantity == 0)
                {
                    await _data.DeleteCartItem(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    cartItem.Price = cartItem.Quantity * cartItem.Book.Price;
                    await _data.UpdateCartItem(cartItem);
                }
            }
            var updatedCart = await _data.GetCartItemsAsync(userId);
            return PartialView("_Cart", updatedCart.ToList());
        }

        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _data.GetCartItemsAsync(userId);
            var cartItem = cartItems.FirstOrDefault(c => c.BookId == id);
            if (cartItem != null)
            {
                await _data.DeleteCartItem(cartItem);
            }
            var updatedCart = await _data.GetCartItemsAsync(userId);

            return PartialView("_Cart", updatedCart.ToList());
        }
    }
}

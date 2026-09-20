using AprilBookStore.DataAccess;
using AprilBookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AprilBookStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IData _data;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(IData data, UserManager<ApplicationUser> userManager)
        {
            _data = data;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _data.GetOrders(User);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _data.GetOrderDetails(id);
            return View(order);
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var order = await _data.SubmitOrder(user.Id);
            if (order == null)
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(order);
        }
    }
}

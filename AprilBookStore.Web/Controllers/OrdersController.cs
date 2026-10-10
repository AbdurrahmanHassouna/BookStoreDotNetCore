using AprilBookStore.Application.Interfaces;
using AprilBookStore.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var orders = await _orderService.GetUserOrdersAsync(userId);
        var viewModels = orders.Select(o => o.ToViewModel()).ToList();

        return View(viewModels);
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmOrder()
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

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
    }
}

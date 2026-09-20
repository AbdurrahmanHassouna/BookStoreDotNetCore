using System.Security.Claims;
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Web.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        var viewModels = orders.Select(o => new OrderListItemViewModel
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            ItemCount = o.OrderItems?.Sum(i => i.Quantity) ?? 0
        }).ToList();

        return View(viewModels);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var viewModel = new OrderDetailsViewModel
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Items = order.OrderItems.Select(item => new OrderItemViewModel
            {
                Id = item.Id,
                BookName = item.BookName,
                Quantity = item.Quantity,
                UnitPrice = item.Price,
                TotalPrice = item.Price * item.Quantity
            }).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> ConfirmOrder()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        try
        {
            var order = await _orderService.SubmitOrderAsync(userId);
            if (order == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new OrderDetailsViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(item => new OrderItemViewModel
                {
                    Id = item.Id,
                    BookName = item.BookName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TotalPrice = item.Price * item.Quantity
                }).ToList()
            };

            return View(viewModel);
        }
        catch (InvalidOperationException ex)
        {
            TempData["CartError"] = ex.Message;
            return RedirectToAction("Index", "Cart");
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["CartError"] = "One or more items in your cart had stock changes while processing your order. Please review your cart and try again.";
            return RedirectToAction("Index", "Cart");
        }
    }
}


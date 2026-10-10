using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Orders;
using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;

    public OrderService(IApplicationDbContext context)
    {
        _context = context;
    }

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

    public async Task<CheckoutResultDto> SubmitOrderAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return CheckoutResultDto.Failed("User is not authenticated.");
        }

        try
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId && !c.IsDeleted)
                    .Include(c => c.Book)
                    .ToListAsync();

                if (cartItems.Count == 0)
                {
                    return CheckoutResultDto.Failed("Your shopping cart is empty.");
                }

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Book == null || cartItem.Book.IsDeleted || !cartItem.Book.IsVisible || cartItem.Book.QuantityInStock < cartItem.Quantity)
                    {
                        var bookName = cartItem.Book?.Name ?? "Selected book";
                        return CheckoutResultDto.Failed($"Insufficient stock for book: '{bookName}'.");
                    }
                }

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

                _context.CartItems.RemoveRange(cartItems);
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                var orderDetailsDto = new OrderDetailsDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        BookName = oi.BookName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.Price
                    }).ToList()
                };

                return CheckoutResultDto.Succeeded(orderDetailsDto);
            });
        }
        catch (DbUpdateConcurrencyException)
        {
            return CheckoutResultDto.Failed("One or more items in your cart had stock changes while processing your order. Please review your cart and try again.");
        }
    }
}

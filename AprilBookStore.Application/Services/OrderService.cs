using AprilBookStore.Application.Interfaces;
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

    public async Task<ICollection<Order>> GetOrdersByUserIdAsync(string userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderDetailsAsync(Guid orderId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> SubmitOrderAsync(string userId)
    {
        return await _context.ExecuteInTransactionAsync(async () =>
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return null;
            }

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItems.Sum(c => (c.Book?.Price ?? c.Price) * c.Quantity),
                UserId = userId,
                OrderItems = new List<OrderItem>()
            };

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Book == null) continue;

                var orderItem = new OrderItem
                {
                    BookName = cartItem.Book.Name,
                    Price = cartItem.Book.Price,
                    Quantity = cartItem.Quantity
                };

                cartItem.Book.QuantityInStock -= cartItem.Quantity;
                if (cartItem.Book.QuantityInStock < 0)
                {
                    throw new InvalidOperationException($"Insufficient stock for book: '{cartItem.Book.Name}'.");
                }

                _context.Books.Update(cartItem.Book);
                order.OrderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.Orders.AddAsync(order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("One or more items in your cart had stock changes while processing your order. Please review your cart and try again.");
            }

            return order;
        });
    }
}

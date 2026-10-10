using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Cart;
using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Services;

public class CartService : ICartService
{
    private readonly IApplicationDbContext _context;

    public CartService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetCartItemsCountAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return 0;
        return await _context.CartItems
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Where(c => c.Book != null && !c.Book.IsDeleted && c.Book.IsVisible)
            .SumAsync(c => (int?)c.Quantity) ?? 0;
    }

    public async Task<CartDto> GetCartAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new CartDto();
        }

        var items = await _context.CartItems
            .AsNoTracking()
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Where(c => c.Book != null && !c.Book.IsDeleted && c.Book.IsVisible)
            .Select(c => new CartItemDto
            {
                Id = c.Id,
                BookId = c.BookId,
                BookName = c.Book!.Name,
                ImgPath = c.Book.ImgPath,
                UnitPrice = c.Book.Price,
                Quantity = c.Quantity,
                QuantityInStock = c.Book.QuantityInStock
            })
            .ToListAsync();

        return new CartDto { Items = items };
    }

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

    public async Task<CartDto> UpdateQuantityAsync(string userId, Guid bookId, int quantity)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new CartDto();
        }

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

        return await GetCartAsync(userId);
    }

    public async Task<CartDto> RemoveItemAsync(string userId, Guid bookId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return new CartDto();
        }

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId && !c.IsDeleted);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return await GetCartAsync(userId);
    }
}

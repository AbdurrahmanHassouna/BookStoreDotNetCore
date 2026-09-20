using AprilBookStore.Application.Interfaces;
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
                .Where(c => c.UserId == userId)
                .SumAsync(c => (int?)c.Quantity) ?? 0;
        }

        public async Task<ICollection<CartItem>> GetCartItemsAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();
        }

        public async Task<int> AddToCartAsync(string userId, Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return await GetCartItemsCountAsync(userId);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);

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

        public async Task<int> UpdateCartItemQuantityAsync(string userId, Guid bookId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);

            if (cartItem != null)
            {
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

            return await GetCartItemsCountAsync(userId);
        }

        public async Task<int> DeleteCartItemAsync(string userId, Guid bookId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return await GetCartItemsCountAsync(userId);
        }
    }


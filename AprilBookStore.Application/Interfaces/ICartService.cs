using AprilBookStore.Domain.Entities;

namespace AprilBookStore.Application.Interfaces;
    public interface ICartService
    {
        Task<int> GetCartItemsCountAsync(string userId);
        Task<ICollection<CartItem>> GetCartItemsAsync(string userId);
        Task<int> AddToCartAsync(string userId, Guid bookId);
        Task<int> UpdateCartItemQuantityAsync(string userId, Guid bookId, int quantity);
        Task<int> DeleteCartItemAsync(string userId, Guid bookId);
    }


using AprilBookStore.Application.Models.Cart;

namespace AprilBookStore.Application.Interfaces;

public interface ICartService
{
    Task<int> GetCartItemsCountAsync(string userId);
    Task<CartDto> GetCartAsync(string userId);
    Task<int> AddToCartAsync(string userId, Guid bookId);
    Task<CartDto> UpdateQuantityAsync(string userId, Guid bookId, int quantity);
    Task<CartDto> RemoveItemAsync(string userId, Guid bookId);
}

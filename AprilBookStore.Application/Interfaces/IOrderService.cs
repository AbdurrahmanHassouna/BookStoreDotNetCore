using AprilBookStore.Domain.Entities;

namespace AprilBookStore.Application.Interfaces;
    public interface IOrderService
    {
        Task<ICollection<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderDetailsAsync(Guid orderId);
        Task<Order?> SubmitOrderAsync(string userId);
    }


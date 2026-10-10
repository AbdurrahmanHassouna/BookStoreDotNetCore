using AprilBookStore.Application.Models.Orders;

namespace AprilBookStore.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderSummaryDto>> GetUserOrdersAsync(string userId);
    Task<OrderDetailsDto?> GetOrderDetailsAsync(Guid orderId, string userId);
    Task<CheckoutResultDto> SubmitOrderAsync(string userId);
}

namespace AprilBookStore.Application.Models.Orders;

public class CheckoutResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public OrderDetailsDto? Order { get; set; }

    public static CheckoutResultDto Succeeded(OrderDetailsDto order) => new()
    {
        Success = true,
        Order = order
    };

    public static CheckoutResultDto Failed(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}

namespace AprilBookStore.Application.Models.Orders;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public string BookName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

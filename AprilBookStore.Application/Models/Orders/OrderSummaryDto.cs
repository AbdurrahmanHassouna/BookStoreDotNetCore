namespace AprilBookStore.Application.Models.Orders;

public class OrderSummaryDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

namespace AprilBookStore.Application.Models.Cart;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string? ImgPath { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int QuantityInStock { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}

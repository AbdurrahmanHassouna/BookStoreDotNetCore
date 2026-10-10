namespace AprilBookStore.Application.Models.Cart;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalItems => Items.Sum(x => x.Quantity);
    public decimal TotalAmount => Items.Sum(x => x.Subtotal);
    public bool IsEmpty => Items.Count == 0;
}

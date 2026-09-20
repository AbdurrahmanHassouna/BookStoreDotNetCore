namespace AprilBookStore.Web.ViewModels.Cart;

public class CartViewModel
{
    public IList<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    public decimal Subtotal => Items.Sum(x => x.Subtotal);
    public decimal TotalAmount => Subtotal;
    public bool IsEmpty => Items.Count == 0;
}

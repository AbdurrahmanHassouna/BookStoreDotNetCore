using Microsoft.AspNetCore.Identity;

namespace AprilBookStore.Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public DateTime BirthDate { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

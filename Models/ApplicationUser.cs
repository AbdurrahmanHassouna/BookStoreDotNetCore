using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AprilBookStore.Models
{
    public class ApplicationUser:IdentityUser
    {
        [InverseProperty("User")]
        public ICollection<CartItem> CartItems { get; set; }

        [InverseProperty("User")]
        public ICollection<Order> Orders { get; set; }
        
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }


    }
}

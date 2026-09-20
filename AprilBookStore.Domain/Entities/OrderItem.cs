using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AprilBookStore.Domain.Entities;
    public class OrderItem : Entity
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public string BookName { get; set; } = null!;

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
    }


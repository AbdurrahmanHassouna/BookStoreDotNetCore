using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Domain.Entities;
    public class Category : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }


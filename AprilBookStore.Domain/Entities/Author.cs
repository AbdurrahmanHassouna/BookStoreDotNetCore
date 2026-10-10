using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Domain.Entities;
    public class Author : Entity
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }


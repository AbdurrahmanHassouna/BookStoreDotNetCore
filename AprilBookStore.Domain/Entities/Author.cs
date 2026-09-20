using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Domain.Entities;
    public class Author : Entity
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }


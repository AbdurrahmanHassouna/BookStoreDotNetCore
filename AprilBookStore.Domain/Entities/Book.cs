using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AprilBookStore.Domain.Entities;

public class Book : Entity
{
    [Required]
    [StringLength(250)]
    public string Name { get; set; } = null!;

    [Required]
    public string Format { get; set; } = null!;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$", ErrorMessage = "Invalid ISBN format. Must be a valid 10 or 13 digit ISBN.")]
    public string ISBN { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string? ImgPath { get; set; }

    public Guid CategoryId { get; set; }
    public Guid AuthorId { get; set; }

    [Range(0.0, 5.0)]
    [Column(TypeName = "decimal(2,1)")]
    public decimal? BookStar { get; set; }

    [Column(TypeName = "date")]
    public DateTime PublicationDate { get; set; }

    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock cannot be negative.")]
    public int QuantityInStock { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public virtual Author? Author { get; set; }
    public virtual Category? Category { get; set; }
}

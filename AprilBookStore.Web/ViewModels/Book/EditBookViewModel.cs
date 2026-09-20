using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Book;

public class EditBookViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Rating"), Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5 stars.")]
    public decimal? BookStar { get; set; }

    [Required, Display(Name = "Book Name")]
    public string Name { get; set; } = string.Empty;
    
    public string Format { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "ISBN is required.")]
    [StringLength(20)]
    [RegularExpression(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$", ErrorMessage = "Invalid ISBN format. Must be a valid 10 or 13 digit ISBN.")]
    public string ISBN { get; set; } = string.Empty;
    
    public decimal Price { get; set; }

    [Display(Name = "Category")]
    public Guid CategoryId { get; set; }

    [Required, Display(Name = "Author")]
    public Guid AuthorId { get; set; }

    [Display(Name = "Publication Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime PublicationDate { get; set; }

    [Display(Name = "Quantity In Stock")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock cannot be negative.")]
    public int QuantityInStock { get; set; }

    [Display(Name = "Book Cover")]
    public IFormFile? BookCoverFile { get; set; }

    public string? Description { get; set; }

    [Display(Name = "Visible")]
    public bool IsVisible { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AprilBookStore.Web.ViewModels.Book;

public class CreateBookViewModel
{
    [Display(Name = "Rating"), Range(0.0, 5.0, ErrorMessage = "Rating must be between 0 and 5 stars.")]
    public decimal? BookStar { get; set; }

    [Required, Display(Name = "Book Name")]
    [Remote("Name", "Books", ErrorMessage = "There is a book with the same Name")]
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
    public DateTime PublicationDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Quantity In Stock")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock cannot be negative.")]
    public int QuantityInStock { get; set; }

    [Display(Name = "Book Cover")]
    public IFormFile? BookCoverFile { get; set; }

    public string? Description { get; set; }
    public SelectList? selectListItems { get; set; }
}

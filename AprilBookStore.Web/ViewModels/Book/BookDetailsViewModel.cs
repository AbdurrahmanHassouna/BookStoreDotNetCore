using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Book;

public class BookDetailsViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Book Name")]
    public string Name { get; set; } = string.Empty;

    public string Format { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    public string? ImgPath { get; set; }

    public Guid? AuthorId { get; set; }

    [Display(Name = "Author")]
    public string AuthorName { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Rating")]
    public decimal? BookStar { get; set; }

    [Display(Name = "Publication Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime PublicationDate { get; set; }

    public string? Description { get; set; }

    [Display(Name = "Quantity In Stock")]
    public int QuantityInStock { get; set; }

    public bool IsVisible { get; set; }
}

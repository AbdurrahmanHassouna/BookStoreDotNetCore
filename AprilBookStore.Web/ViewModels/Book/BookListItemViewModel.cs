using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Book;

public class BookListItemViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Book Name")]
    public string Name { get; set; } = string.Empty;

    public string Format { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Quantity In Stock")]
    public int QuantityInStock { get; set; }

    [Display(Name = "Deleted")]
    public bool IsDeleted { get; set; }

    [Display(Name = "Visible")]
    public bool IsVisible { get; set; }

    [Display(Name = "Author")]
    public string AuthorName { get; set; } = string.Empty;

    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;
}

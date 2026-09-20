using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Author;

public class AuthorViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Author Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Visible")]
    public bool IsVisible { get; set; }

    [Display(Name = "Published Books")]
    public int BookCount { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Updated Date")]
    public DateTime UpdatedDate { get; set; }

    public List<AuthorBookItemViewModel> Books { get; set; } = new();
}

public class AuthorBookItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImgPath { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal? BookStar { get; set; }
    public int QuantityInStock { get; set; }
    public bool IsVisible { get; set; }
}

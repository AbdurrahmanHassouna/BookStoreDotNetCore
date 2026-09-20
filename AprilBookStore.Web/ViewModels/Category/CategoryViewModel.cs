using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Category;

public class CategoryViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Visible")]
    public bool IsVisible { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Updated Date")]
    public DateTime UpdatedDate { get; set; }

    [Display(Name = "Books in Category")]
    public int BookCount { get; set; }

    public List<CategoryBookItemViewModel> Books { get; set; } = new();
}

public class CategoryBookItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImgPath { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public decimal? BookStar { get; set; }
    public int QuantityInStock { get; set; }
    public bool IsVisible { get; set; }
}

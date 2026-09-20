using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Category;

public class CreateCategoryViewModel
{
    [Required(ErrorMessage = "Category name is required.")]
    [Display(Name = "Category Name")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Visible")]
    public bool IsVisible { get; set; } = true;
}

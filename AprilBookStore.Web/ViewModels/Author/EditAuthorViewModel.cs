using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Author;

public class EditAuthorViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Author name is required.")]
    [Display(Name = "Author Name")]
    [StringLength(150, ErrorMessage = "Author name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Visible in catalog")]
    public bool IsVisible { get; set; } = true;
}

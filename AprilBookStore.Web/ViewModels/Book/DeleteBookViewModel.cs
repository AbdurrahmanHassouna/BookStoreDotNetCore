using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.Book;

public class DeleteBookViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Book Name")]
    public string Name { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    [Display(Name = "Author")]
    public string AuthorName { get; set; } = string.Empty;

    public string? ImgPath { get; set; }
}

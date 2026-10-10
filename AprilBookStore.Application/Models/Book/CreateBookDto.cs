using AprilBookStore.Application.Models.Common;

namespace AprilBookStore.Application.Models.Book;

public class CreateBookDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal? BookStar { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime PublicationDate { get; set; }
    public int QuantityInStock { get; set; }
    public FileUploadDto? CoverImage { get; set; }
}

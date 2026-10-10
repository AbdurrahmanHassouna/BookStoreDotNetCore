namespace AprilBookStore.Application.Models.Book;

public class BookDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImgPath { get; set; }
    public Guid? AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal? BookStar { get; set; }
    public DateTime PublicationDate { get; set; }
    public string? Description { get; set; }
    public int QuantityInStock { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDeleted { get; set; }
}

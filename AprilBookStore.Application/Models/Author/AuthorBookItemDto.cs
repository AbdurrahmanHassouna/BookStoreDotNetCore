namespace AprilBookStore.Application.Models.Author;

public class AuthorBookItemDto
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

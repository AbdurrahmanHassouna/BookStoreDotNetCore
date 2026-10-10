namespace AprilBookStore.Application.Models.Category;

public class CategoryBookItemDto
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

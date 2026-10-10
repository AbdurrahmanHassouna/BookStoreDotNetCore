namespace AprilBookStore.Application.Models.Book;

public class BookListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsVisible { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

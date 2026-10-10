namespace AprilBookStore.Application.Models.Category;

public class CategoryDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public int BookCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public List<CategoryBookItemDto> Books { get; set; } = new();
}

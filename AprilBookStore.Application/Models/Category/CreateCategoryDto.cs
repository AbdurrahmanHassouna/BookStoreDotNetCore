namespace AprilBookStore.Application.Models.Category;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

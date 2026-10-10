namespace AprilBookStore.Application.Models.Author;

public class CreateAuthorDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

namespace AprilBookStore.Application.Models.Author;

public class UpdateAuthorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

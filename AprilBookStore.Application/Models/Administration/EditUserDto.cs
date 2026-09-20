namespace AprilBookStore.Application.Models.Administration;

public class EditUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public List<string> Claims { get; set; } = new();
}

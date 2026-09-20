namespace AprilBookStore.Application.Models.Administration;

public class EditRoleDto
{
    public string Id { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public List<string> Users { get; set; } = new();
}

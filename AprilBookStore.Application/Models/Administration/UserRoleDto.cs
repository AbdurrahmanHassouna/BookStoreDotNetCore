namespace AprilBookStore.Application.Models.Administration;

public class UserRoleDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

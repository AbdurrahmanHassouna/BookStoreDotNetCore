namespace AprilBookStore.Application.Models.Administration;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString() => UserName;
}

namespace AprilBookStore.Application.Models.Administration;

public class UserClaimsDto
{
    public string UserId { get; set; } = string.Empty;
    public List<UserClaimDto> Claims { get; set; } = new();
}

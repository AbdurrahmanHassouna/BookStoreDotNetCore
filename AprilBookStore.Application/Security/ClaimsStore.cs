using System.Security.Claims;

namespace AprilBookStore.Application.Security;

public static class ClaimsStore
{
    public static readonly List<Claim> AllClaims = new List<Claim>
    {
        new Claim("Create Role", "Create Role"),
        new Claim("Edit Role", "Edit Role"),
        new Claim("Delete Role", "Delete Role")
    };
}

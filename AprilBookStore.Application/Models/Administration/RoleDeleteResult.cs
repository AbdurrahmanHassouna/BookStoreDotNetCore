using AprilBookStore.Application.Models.Common;

namespace AprilBookStore.Application.Models.Administration;

public class RoleDeleteResult : ServiceResult
{
    public bool IsInUse { get; private set; }
    public string? RoleName { get; private set; }

    public static RoleDeleteResult Success(string? roleName = null) => new()
    {
        Succeeded = true,
        RoleName = roleName
    };

    public static RoleDeleteResult InUse(string roleName, params string[] errors) => new()
    {
        Succeeded = false,
        IsInUse = true,
        RoleName = roleName,
        Errors = errors.ToList()
    };

    public static RoleDeleteResult InUse(string roleName, IEnumerable<string> errors) => new()
    {
        Succeeded = false,
        IsInUse = true,
        RoleName = roleName,
        Errors = errors.ToList()
    };

    public static RoleDeleteResult RoleNotFound(string id) => new()
    {
        Succeeded = false,
        IsInUse = false,
        RoleName = null,
        Errors = new List<string> { $"No role found with ID = {id}" }
    };

    public new static RoleDeleteResult Failed(params string[] errors) => new()
    {
        Succeeded = false,
        IsInUse = false,
        Errors = errors.ToList()
    };

    public static RoleDeleteResult Failed(string? roleName, IEnumerable<string> errors) => new()
    {
        Succeeded = false,
        IsInUse = false,
        RoleName = roleName,
        Errors = errors.ToList()
    };
}

using AprilBookStore.Application.Models.Administration;

namespace AprilBookStore.Application.Interfaces;

public interface IAdministrationService
{
    // Roles
    Task<ICollection<RoleDto>> GetRolesAsync();
    Task<(bool Succeeded, IEnumerable<string> Errors)> CreateRoleAsync(string roleName);
    Task<EditRoleDto?> GetRoleForEditAsync(string id);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateRoleAsync(string id, string roleName);
    Task<(bool Succeeded, bool IsInUse, string? RoleName, IEnumerable<string> Errors)> DeleteRoleAsync(string id);
    Task<ICollection<UserRoleDto>> GetUsersInRoleAsync(string roleId);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUsersInRoleAsync(string roleId, List<UserRoleDto> userRoles);

    // Users
    Task<ICollection<UserDto>> GetUsersAsync();
    Task<(bool Succeeded, IEnumerable<string> Errors)> DeleteUserAsync(string id);
    Task<EditUserDto?> GetUserForEditAsync(string id);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserAsync(string id, string email, string userName);
    Task<ICollection<UserRolesDto>> GetUserRolesAsync(string userId);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserRolesAsync(string userId, List<UserRolesDto> roles);
    Task<UserClaimsDto?> GetUserClaimsAsync(string userId);
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateUserClaimsAsync(string userId, List<UserClaimDto> claims);
}

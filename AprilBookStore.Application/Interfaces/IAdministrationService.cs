using AprilBookStore.Application.Models.Administration;
using AprilBookStore.Application.Models.Common;

namespace AprilBookStore.Application.Interfaces;

public interface IAdministrationService
{
    // Roles
    Task<ICollection<RoleDto>> GetRolesAsync();
    Task<ServiceResult> CreateRoleAsync(string roleName);
    Task<EditRoleDto?> GetRoleForEditAsync(string id);
    Task<ServiceResult> UpdateRoleAsync(string id, string roleName);
    Task<RoleDeleteResult> DeleteRoleAsync(string id);
    Task<ICollection<UserRoleDto>> GetUsersInRoleAsync(string roleId);
    Task<ServiceResult> UpdateUsersInRoleAsync(string roleId, List<UserRoleDto> userRoles);

    // Users
    Task<ICollection<UserDto>> GetUsersAsync();
    Task<ServiceResult> DeleteUserAsync(string id);
    Task<EditUserDto?> GetUserForEditAsync(string id);
    Task<ServiceResult> UpdateUserAsync(string id, string email, string userName);
    Task<ICollection<UserRolesDto>> GetUserRolesAsync(string userId);
    Task<ServiceResult> UpdateUserRolesAsync(string userId, List<UserRolesDto> roles);
    Task<UserClaimsDto?> GetUserClaimsAsync(string userId);
    Task<ServiceResult> UpdateUserClaimsAsync(string userId, List<UserClaimDto> claims);
}

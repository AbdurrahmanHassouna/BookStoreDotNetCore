using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Administration;
using AprilBookStore.Application.Models.Common;
using AprilBookStore.Application.Security;
using AprilBookStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AprilBookStore.Application.Services;

public class AdministrationService : IAdministrationService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdministrationService> _logger;

    public AdministrationService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AdministrationService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ICollection<RoleDto>> GetRolesAsync()
    {
        return await _roleManager.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();
    }

    public async Task<ServiceResult> CreateRoleAsync(string roleName)
    {
        var identityRole = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(identityRole);
        return result.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Failed(result.Errors.Select(e => e.Description));
    }

    public async Task<EditRoleDto?> GetRoleForEditAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return null;

        var model = new EditRoleDto
        {
            Id = role.Id,
            RoleName = role.Name
        };

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        model.Users = usersInRole.Select(u => u.UserName!).ToList();

        return model;
    }

    public async Task<ServiceResult> UpdateRoleAsync(string id, string roleName)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return ServiceResult.Failed($"No role found with ID = {id}");
        }

        role.Name = roleName;
        var result = await _roleManager.UpdateAsync(role);
        return result.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Failed(result.Errors.Select(e => e.Description));
    }

    public async Task<RoleDeleteResult> DeleteRoleAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return RoleDeleteResult.RoleNotFound(id);
        }

        if (string.Equals(role.Name, "SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RoleDeleteResult.Failed("System roles cannot be deleted.");
        }

        try
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RoleDeleteResult.Success(role.Name);
            }
            return RoleDeleteResult.Failed(role.Name, result.Errors.Select(e => e.Description));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting role {RoleName} ({RoleId})", role.Name, id);
            return RoleDeleteResult.InUse(role.Name!, $"{role.Name} role is in use.");
        }
    }

    public async Task<ICollection<UserRoleDto>> GetUsersInRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return new List<UserRoleDto>();

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        var userInRoleIds = new HashSet<string>(usersInRole.Select(u => u.Id));

        var allUsers = await _userManager.Users.ToListAsync();
        return allUsers.Select(user => new UserRoleDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            IsSelected = userInRoleIds.Contains(user.Id)
        }).ToList();
    }

    public async Task<ServiceResult> UpdateUsersInRoleAsync(string roleId, List<UserRoleDto> userRoles)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return ServiceResult.Failed($"No role found with ID = {roleId}");
        }

        var errors = new List<string>();
        foreach (var item in userRoles)
        {
            var user = await _userManager.FindByIdAsync(item.UserId);
            if (user == null) continue;

            var isInRole = await _userManager.IsInRoleAsync(user, role.Name!);
            if (item.IsSelected && !isInRole)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                    errors.AddRange(result.Errors.Select(e => e.Description));
            }
            else if (!item.IsSelected && isInRole)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                    errors.AddRange(result.Errors.Select(e => e.Description));
            }
        }

        return errors.Count == 0
            ? ServiceResult.Success()
            : ServiceResult.Failed(errors);
    }

    public async Task<ICollection<UserDto>> GetUsersAsync()
    {
        return await _userManager.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            })
            .ToListAsync();
    }

    public async Task<ServiceResult> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Failed($"No user found with ID = {id}");
        }

        var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
        if (isSuperAdmin)
        {
            return ServiceResult.Failed("SuperAdmin accounts cannot be deleted.");
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Failed(result.Errors.Select(e => e.Description));
    }

    public async Task<EditUserDto?> GetUserForEditAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        return new EditUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles,
            Claims = claims.Select(x => $"{x.Type} :  {x.Value}").ToList()
        };
    }

    public async Task<ServiceResult> UpdateUserAsync(string id, string email, string userName)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Failed($"No user found with ID = {id}");
        }

        user.Email = email;
        user.UserName = userName;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Failed(result.Errors.Select(e => e.Description));
    }

    public async Task<ICollection<UserRolesDto>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<UserRolesDto>();

        var userRoles = await _userManager.GetRolesAsync(user);
        var userRolesSet = new HashSet<string>(userRoles);

        var roles = await _roleManager.Roles.ToListAsync();
        return roles.Select(role => new UserRolesDto
        {
            RoleId = role.Id,
            RoleName = role.Name!,
            IsSelected = userRolesSet.Contains(role.Name!)
        }).ToList();
    }

    public async Task<ServiceResult> UpdateUserRolesAsync(string userId, List<UserRolesDto> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Failed($"No user found with ID = {userId}");
        }

        var errors = new List<string>();
        foreach (var item in roles)
        {
            var role = await _roleManager.FindByIdAsync(item.RoleId);
            if (role == null) continue;

            var isInRole = await _userManager.IsInRoleAsync(user, role.Name!);
            if (item.IsSelected && !isInRole)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                    errors.AddRange(result.Errors.Select(e => e.Description));
            }
            else if (!item.IsSelected && isInRole)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                    errors.AddRange(result.Errors.Select(e => e.Description));
            }
        }

        return errors.Count == 0
            ? ServiceResult.Success()
            : ServiceResult.Failed(errors);
    }

    public async Task<UserClaimsDto?> GetUserClaimsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var existingUserClaims = await _userManager.GetClaimsAsync(user);
        var dto = new UserClaimsDto
        {
            UserId = user.Id
        };

        foreach (var claim in ClaimsStore.AllClaims)
        {
            dto.Claims.Add(new UserClaimDto
            {
                ClaimType = claim.Type,
                IsSelected = existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true")
            });
        }

        return dto;
    }

    public async Task<ServiceResult> UpdateUserClaimsAsync(string userId, List<UserClaimDto> claims)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return ServiceResult.Failed($"No user found with ID = {userId}");
        }

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var removeResult = await _userManager.RemoveClaimsAsync(user, existingClaims);
        if (!removeResult.Succeeded)
        {
            return ServiceResult.Failed(removeResult.Errors.Select(e => e.Description));
        }

        var newClaims = claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false"));
        var addResult = await _userManager.AddClaimsAsync(user, newClaims);
        return addResult.Succeeded
            ? ServiceResult.Success()
            : ServiceResult.Failed(addResult.Errors.Select(e => e.Description));
    }
}

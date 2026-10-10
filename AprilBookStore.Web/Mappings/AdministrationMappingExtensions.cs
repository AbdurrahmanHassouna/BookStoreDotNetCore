using AprilBookStore.Application.Models.Administration;
using AprilBookStore.Web.ViewModels.User;

namespace AprilBookStore.Web.Mappings;

public static class AdministrationMappingExtensions
{
    public static EditRoleViewModel ToViewModel(this EditRoleDto dto)
    {
        return new EditRoleViewModel
        {
            Id = dto.Id,
            RoleName = dto.RoleName,
            Users = dto.Users ?? new List<string>()
        };
    }

    public static UserRoleViewModel ToViewModel(this UserRoleDto dto)
    {
        return new UserRoleViewModel
        {
            UserId = dto.UserId,
            UserName = dto.UserName,
            IsSelected = dto.IsSelected
        };
    }

    public static List<UserRoleViewModel> ToViewModelList(this IEnumerable<UserRoleDto> dtos)
    {
        return dtos.Select(ToViewModel).ToList();
    }

    public static UserRoleDto ToDto(this UserRoleViewModel vm)
    {
        return new UserRoleDto
        {
            UserId = vm.UserId,
            UserName = vm.UserName,
            IsSelected = vm.IsSelected
        };
    }

    public static List<UserRoleDto> ToDtoList(this IEnumerable<UserRoleViewModel> vms)
    {
        return vms.Select(ToDto).ToList();
    }

    public static EditUserViewModel ToViewModel(this EditUserDto dto)
    {
        return new EditUserViewModel
        {
            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email,
            Roles = dto.Roles ?? new List<string>(),
            Claims = dto.Claims ?? new List<string>()
        };
    }

    public static UserRolesViewModel ToViewModel(this UserRolesDto dto)
    {
        return new UserRolesViewModel
        {
            RoleId = dto.RoleId,
            RoleName = dto.RoleName,
            IsSelected = dto.IsSelected
        };
    }

    public static List<UserRolesViewModel> ToViewModelList(this IEnumerable<UserRolesDto> dtos)
    {
        return dtos.Select(ToViewModel).ToList();
    }

    public static UserRolesDto ToDto(this UserRolesViewModel vm)
    {
        return new UserRolesDto
        {
            RoleId = vm.RoleId,
            RoleName = vm.RoleName,
            IsSelected = vm.IsSelected
        };
    }

    public static List<UserRolesDto> ToDtoList(this IEnumerable<UserRolesViewModel> vms)
    {
        return vms.Select(ToDto).ToList();
    }

    public static UserClaimsViewModel ToViewModel(this UserClaimsDto dto)
    {
        return new UserClaimsViewModel
        {
            UserId = dto.UserId,
            Claims = dto.Claims.Select(c => new UserClaim
            {
                ClaimType = c.ClaimType,
                IsSelected = c.IsSelected
            }).ToList()
        };
    }

    public static List<UserClaimDto> ToDtoList(this UserClaimsViewModel vm)
    {
        return vm.Claims.Select(c => new UserClaimDto
        {
            ClaimType = c.ClaimType,
            IsSelected = c.IsSelected
        }).ToList();
    }

    public static List<UserClaimDto> ToDtoList(this IEnumerable<UserClaim> claims)
    {
        return claims.Select(c => new UserClaimDto
        {
            ClaimType = c.ClaimType,
            IsSelected = c.IsSelected
        }).ToList();
    }
}

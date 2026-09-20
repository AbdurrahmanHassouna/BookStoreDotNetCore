using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Administration;
using AprilBookStore.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AdministrationController : Controller
{
    private readonly IAdministrationService _adminService;
    private readonly ILogger<AdministrationController> _logger;

    public AdministrationController(
        IAdministrationService adminService,
        ILogger<AdministrationController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var (succeeded, errors) = await _adminService.CreateRoleAsync(model.RoleName);
            if (succeeded)
            {
                return RedirectToAction("ListRoles");
            }
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ListRoles()
    {
        var roles = await _adminService.GetRolesAsync();
        return View(roles);
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditRole(string id)
    {
        var role = await _adminService.GetRoleForEditAsync(id);
        if (role == null)
        {
            ViewBag.ErrorMessage = $"no role with this id = {id}";
            return View("NotFound");
        }

        var model = new EditRoleViewModel
        {
            Id = role.Id,
            RoleName = role.RoleName,
            Users = role.Users
        };
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditRole(EditRoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (succeeded, errors) = await _adminService.UpdateRoleAsync(model.Id, model.RoleName);
        if (succeeded)
        {
            return RedirectToAction("ListRoles");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError("", error);
        }
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "DeleteRolePolicy")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var (succeeded, isInUse, roleName, errors) = await _adminService.DeleteRoleAsync(id);
        if (succeeded)
        {
            return RedirectToAction("ListRoles");
        }
        if (roleName == null)
        {
            ViewBag.ErrorMessage = $"No role found with ID: {id}";
            return View("NotFound");
        }
        if (isInUse)
        {
            ViewBag.ErrorTitle = $"{roleName} role is in use";
            ViewBag.ErrorMessage = $"{roleName} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
            ViewBag.RoleId = id;
            return View("Error");
        }
        foreach (var error in errors)
        {
            ModelState.AddModelError("", error);
        }
        return View("ListRoles", await _adminService.GetRolesAsync());
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUsersInRole(string id)
    {
        ViewBag.roleId = id;
        var usersInRole = await _adminService.GetUsersInRoleAsync(id);
        var model = usersInRole.Select(u => new UserRoleViewModel
        {
            UserId = u.UserId,
            UserName = u.UserName,
            IsSelected = u.IsSelected
        }).ToList();

        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> userRoles, string id)
    {
        var dtoList = userRoles.Select(u => new UserRoleDto
        {
            UserId = u.UserId,
            UserName = u.UserName,
            IsSelected = u.IsSelected
        }).ToList();

        var (succeeded, errors) = await _adminService.UpdateUsersInRoleAsync(id, dtoList);
        if (!succeeded)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        return RedirectToAction("EditRole", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        var users = await _adminService.GetUsersAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var (succeeded, errors) = await _adminService.DeleteUserAsync(id);
        if (succeeded)
        {
            return RedirectToAction("ListUsers");
        }
        foreach (var error in errors)
        {
            ModelState.AddModelError("", error);
        }
        return View("ListUsers", await _adminService.GetUsersAsync());
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _adminService.GetUserForEditAsync(id);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"no User with this id = {id}";
            return View("NotFound");
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = user.Roles,
            Claims = user.Claims
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (succeeded, errors) = await _adminService.UpdateUserAsync(model.Id, model.Email, model.UserName);
        if (succeeded)
        {
            return RedirectToAction("ListUsers");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError("", error);
        }
        return View(model);
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUserRoles(string id)
    {
        ViewBag.userId = id;
        var userRoles = await _adminService.GetUserRolesAsync(id);
        var model = userRoles.Select(r => new UserRolesViewModel
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            IsSelected = r.IsSelected
        }).ToList();

        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUserRoles(List<UserRolesViewModel> models, string id)
    {
        var dtoList = models.Select(r => new UserRolesDto
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            IsSelected = r.IsSelected
        }).ToList();

        var (succeeded, errors) = await _adminService.UpdateUserRolesAsync(id, dtoList);
        if (!succeeded)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        return RedirectToAction("EditUser", new { id });
    }

    [HttpGet]
    public async Task<IActionResult> EditUserClaims(string id)
    {
        var userClaimsDto = await _adminService.GetUserClaimsAsync(id);
        if (userClaimsDto == null)
        {
            ViewBag.ErrorMessage = $"no user with this id = {id}";
            return View("NotFound");
        }

        var model = new UserClaimsViewModel
        {
            UserId = userClaimsDto.UserId,
            Claims = userClaimsDto.Claims.Select(c => new UserClaim
            {
                ClaimType = c.ClaimType,
                IsSelected = c.IsSelected
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUserClaims(UserClaimsViewModel model, string id)
    {
        var dtoList = model.Claims.Select(c => new UserClaimDto
        {
            ClaimType = c.ClaimType,
            IsSelected = c.IsSelected
        }).ToList();

        var (succeeded, errors) = await _adminService.UpdateUserClaimsAsync(id, dtoList);
        if (succeeded)
        {
            return RedirectToAction("EditUser", new { id });
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError("", error);
        }
        return View(model);
    }
}
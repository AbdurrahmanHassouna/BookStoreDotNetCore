using AprilBookStore.Application.Interfaces;
using AprilBookStore.Web.Mappings;
using AprilBookStore.Web.ViewModels.Error;
using AprilBookStore.Web.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AdministrationController : Controller
{
    private readonly IAdministrationService _adminService;

    public AdministrationController(IAdministrationService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _adminService.CreateRoleAsync(model.RoleName);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ListRoles));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
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
            return View("NotFound", new ErrorViewModel
            {
                StatusCode = 404,
                ErrorTitle = "Role Not Found",
                ErrorMessage = $"No role found with ID = {id}"
            });
        }

        return View(role.ToViewModel());
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(EditRoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.UpdateRoleAsync(model.Id, model.RoleName);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ListRoles));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "DeleteRolePolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var result = await _adminService.DeleteRoleAsync(id);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ListRoles));
        }
        if (result.RoleName == null)
        {
            return View("NotFound", new ErrorViewModel
            {
                StatusCode = 404,
                ErrorTitle = "Role Not Found",
                ErrorMessage = $"No role found with ID: {id}"
            });
        }
        if (result.IsInUse)
        {
            var errorModel = new ErrorViewModel
            {
                ErrorTitle = $"{result.RoleName} role is in use",
                ErrorMessage = $"{result.RoleName} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete",
                RoleId = id
            };
            return View("Error", errorModel);
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return View("ListRoles", await _adminService.GetRolesAsync());
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUsersInRole(string id)
    {
        ViewBag.roleId = id;
        var usersInRole = await _adminService.GetUsersInRoleAsync(id);
        return View(usersInRole.ToViewModelList());
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> userRoles, string id)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.roleId = id;
            return View(userRoles);
        }

        var result = await _adminService.UpdateUsersInRoleAsync(id, userRoles.ToDtoList());
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            ViewBag.roleId = id;
            return View(userRoles);
        }

        return RedirectToAction(nameof(EditRole), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        var users = await _adminService.GetUsersAsync();
        return View(users);
    }

    [HttpPost]
    [Authorize(Policy = "DeleteUserPolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _adminService.DeleteUserAsync(id);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ListUsers));
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return View("ListUsers", await _adminService.GetUsersAsync());
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _adminService.GetUserForEditAsync(id);
        if (user == null)
        {
            return View("NotFound", new ErrorViewModel
            {
                StatusCode = 404,
                ErrorTitle = "User Not Found",
                ErrorMessage = $"No user found with ID = {id}"
            });
        }

        return View(user.ToViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.UpdateUserAsync(model.Id, model.Email, model.UserName);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ListUsers));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return View(model);
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUserRoles(string id)
    {
        ViewBag.userId = id;
        var userRoles = await _adminService.GetUserRolesAsync(id);
        return View(userRoles.ToViewModelList());
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserRoles(List<UserRolesViewModel> models, string id)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.userId = id;
            return View(models);
        }

        var result = await _adminService.UpdateUserRolesAsync(id, models.ToDtoList());
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            ViewBag.userId = id;
            return View(models);
        }

        return RedirectToAction(nameof(EditUser), new { id });
    }

    [HttpGet]
    [Authorize(Policy = "EditRolePolicy")]
    public async Task<IActionResult> EditUserClaims(string id)
    {
        var userClaimsDto = await _adminService.GetUserClaimsAsync(id);
        if (userClaimsDto == null)
        {
            return View("NotFound", new ErrorViewModel
            {
                StatusCode = 404,
                ErrorTitle = "User Not Found",
                ErrorMessage = $"No user found with ID = {id}"
            });
        }

        return View(userClaimsDto.ToViewModel());
    }

    [HttpPost]
    [Authorize(Policy = "EditRolePolicy")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUserClaims(UserClaimsViewModel model, string id)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _adminService.UpdateUserClaimsAsync(id, model.ToDtoList());
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(EditUser), new { id });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return View(model);
    }
}
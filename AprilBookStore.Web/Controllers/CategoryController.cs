using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Category;
using AprilBookStore.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    private bool IsAdminUser() => User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetCategoriesAsync(forAdmin: IsAdminUser());
        var model = categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            IsVisible = c.IsVisible,
            BookCount = c.BookCount,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate
        }).ToList();

        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var category = await _categoryService.GetCategoryDetailsAsync(id, forAdmin: IsAdminUser());
        if (category == null)
        {
            return NotFound();
        }

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsVisible = category.IsVisible,
            BookCount = category.BookCount,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            Books = category.Books.Select(b => new CategoryBookItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Price = b.Price,
                ImgPath = b.ImgPath,
                AuthorName = b.AuthorName,
                BookStar = b.BookStar,
                QuantityInStock = b.QuantityInStock,
                IsVisible = b.IsVisible
            }).ToList()
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new CreateCategoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new CreateCategoryDto
        {
            Name = model.Name,
            IsVisible = model.IsVisible
        };

        var result = await _categoryService.CreateCategoryAsync(dto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var model = new EditCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsVisible = category.IsVisible
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditCategoryViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new UpdateCategoryDto
        {
            Id = model.Id,
            Name = model.Name,
            IsVisible = model.IsVisible
        };

        var result = await _categoryService.UpdateCategoryAsync(dto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsVisible = category.IsVisible,
            BookCount = category.BookCount,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                IsVisible = category.IsVisible,
                BookCount = category.BookCount,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate
            };
            return View("Delete", model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        var result = await _categoryService.RestoreCategoryAsync(id);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(", ", result.Errors);
        }
        else
        {
            TempData["SuccessMessage"] = "Category restored successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
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

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
        var model = categories.Select(c => new CategoryViewModel
        {
            Id = c.Id,
            Name = c.Name,
            IsVisible = c.IsVisible,
            BookCount = c.Books != null
                ? (isAdmin ? c.Books.Count : c.Books.Count(b => b.IsVisible))
                : 0,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate
        }).ToList();

        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
        var books = category.Books?.AsEnumerable() ?? Enumerable.Empty<Book>();
        if (!isAdmin)
        {
            books = books.Where(b => b.IsVisible);
        }

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsVisible = category.IsVisible,
            BookCount = books.Count(),
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            Books = books.Select(b => new CategoryBookItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Price = b.Price,
                ImgPath = b.ImgPath,
                AuthorName = b.Author?.Name ?? "Unknown Author",
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
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Name = model.Name,
                IsVisible = model.IsVisible
            };

            await _categoryService.CreateCategoryAsync(category);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
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

        if (ModelState.IsValid)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = model.Name;
            category.IsVisible = model.IsVisible;

            await _categoryService.UpdateCategoryAsync(category);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
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
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Author;
using AprilBookStore.Web.ViewModels.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AuthorsController : Controller
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    private bool IsAdminUser() => User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var authors = await _authorService.GetAuthorsAsync(forAdmin: IsAdminUser());
        var model = authors.Select(a => new AuthorViewModel
        {
            Id = a.Id,
            Name = a.Name,
            IsVisible = a.IsVisible,
            BookCount = a.BookCount,
            CreatedDate = a.CreatedDate,
            UpdatedDate = a.UpdatedDate
        }).ToList();

        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var author = await _authorService.GetAuthorDetailsAsync(id, forAdmin: IsAdminUser());
        if (author == null)
        {
            return NotFound();
        }

        var model = new AuthorViewModel
        {
            Id = author.Id,
            Name = author.Name,
            IsVisible = author.IsVisible,
            BookCount = author.BookCount,
            CreatedDate = author.CreatedDate,
            UpdatedDate = author.UpdatedDate,
            Books = author.Books.Select(b => new AuthorBookItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Price = b.Price,
                ImgPath = b.ImgPath,
                CategoryName = b.CategoryName,
                BookStar = b.BookStar,
                QuantityInStock = b.QuantityInStock,
                IsVisible = b.IsVisible
            }).ToList()
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new CreateAuthorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAuthorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new CreateAuthorDto
        {
            Name = model.Name,
            IsVisible = model.IsVisible
        };

        var result = await _authorService.CreateAuthorAsync(dto);
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
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        var model = new EditAuthorViewModel
        {
            Id = author.Id,
            Name = author.Name,
            IsVisible = author.IsVisible
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditAuthorViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var dto = new UpdateAuthorDto
        {
            Id = model.Id,
            Name = model.Name,
            IsVisible = model.IsVisible
        };

        var result = await _authorService.UpdateAuthorAsync(dto);
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
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        var model = new AuthorViewModel
        {
            Id = author.Id,
            Name = author.Name,
            IsVisible = author.IsVisible,
            BookCount = author.BookCount,
            CreatedDate = author.CreatedDate,
            UpdatedDate = author.UpdatedDate
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        var result = await _authorService.DeleteAuthorAsync(id);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            var model = new AuthorViewModel
            {
                Id = author.Id,
                Name = author.Name,
                IsVisible = author.IsVisible,
                BookCount = author.BookCount,
                CreatedDate = author.CreatedDate,
                UpdatedDate = author.UpdatedDate
            };
            return View("Delete", model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        var result = await _authorService.RestoreAuthorAsync(id);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(", ", result.Errors);
        }
        else
        {
            TempData["SuccessMessage"] = "Author restored successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}

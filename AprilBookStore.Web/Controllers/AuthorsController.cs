using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
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

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var authors = await _authorService.GetAuthorsAsync();
        var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
        var model = authors.Select(a => new AuthorViewModel
        {
            Id = a.Id,
            Name = a.Name,
            IsVisible = a.IsVisible,
            BookCount = a.Books != null
                ? (isAdmin ? a.Books.Count : a.Books.Count(b => b.IsVisible))
                : 0,
            CreatedDate = a.CreatedDate,
            UpdatedDate = a.UpdatedDate
        }).ToList();

        return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
        var books = author.Books?.AsEnumerable() ?? Enumerable.Empty<Book>();
        if (!isAdmin)
        {
            books = books.Where(b => b.IsVisible);
        }

        var model = new AuthorViewModel
        {
            Id = author.Id,
            Name = author.Name,
            IsVisible = author.IsVisible,
            BookCount = books.Count(),
            CreatedDate = author.CreatedDate,
            UpdatedDate = author.UpdatedDate,
            Books = books.Select(b => new AuthorBookItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Price = b.Price,
                ImgPath = b.ImgPath,
                CategoryName = b.Category?.Name ?? "General",
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
        if (ModelState.IsValid)
        {
            var author = new Author
            {
                Name = model.Name,
                IsVisible = model.IsVisible
            };

            await _authorService.CreateAuthorAsync(author);
            return RedirectToAction(nameof(Index));
        }
        return View(model);
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

        if (!ModelState.IsValid) return View(model);


        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        author.Name = model.Name;
        author.IsVisible = model.IsVisible;

        await _authorService.UpdateAuthorAsync(author);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);

        if (author == null) return NotFound();


        var model = new AuthorViewModel
        {
            Id = author.Id,
            Name = author.Name,
            IsVisible = author.IsVisible,
            BookCount = author.Books?.Count ?? 0,
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

        try
        {
            await _authorService.DeleteAuthorAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var model = new AuthorViewModel
            {
                Id = author.Id,
                Name = author.Name,
                IsVisible = author.IsVisible,
                BookCount = author.Books?.Count ?? 0,
                CreatedDate = author.CreatedDate,
                UpdatedDate = author.UpdatedDate
            };
            return View("Delete", model);
        }
    }
}

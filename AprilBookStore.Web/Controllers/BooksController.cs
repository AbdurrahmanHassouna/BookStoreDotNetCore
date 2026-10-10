using System.Net;
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Book;
using AprilBookStore.Web.Extensions;
using AprilBookStore.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AprilBookStore.Web.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;

    public BooksController(
        IBookService bookService,
        IAuthorService authorService,
        ICategoryService categoryService)
    {
        _bookService = bookService;
        _authorService = authorService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> AutoComplete(string term)
    {
        var result = await _bookService.SearchBooksAsync(term, true);
        return Json(result);
    }

    public async Task<IActionResult> Name(string name)
    {
        bool isUnique = await _bookService.IsBookNameUniqueAsync(name);
        return Json(isUnique);
    }

    [HttpGet]
    public async Task<ActionResult> Index(
        string? name,
        string? search,
        Guid? categoryId,
        string? status,
        decimal? minPrice,
        decimal? maxPrice,
        string? sortOrder,
        string? prevSort,
        int? page)
    {
        var searchTerm = !string.IsNullOrWhiteSpace(name) ? name : search;

        ViewData["NameFilter"] = searchTerm;
        ViewData["CategoryFilter"] = categoryId;
        ViewData["StatusFilter"] = status;
        ViewData["MinPrice"] = minPrice;
        ViewData["MaxPrice"] = maxPrice;
        ViewData["CurrentSort"] = string.IsNullOrWhiteSpace(sortOrder) ? "date_desc" : sortOrder;
        ViewData["PrevSort"] = prevSort;

        ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", categoryId);

        var filter = new BookAdminFilterDto
        {
            Search = searchTerm,
            CategoryId = categoryId,
            Status = status,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortOrder = sortOrder,
            PrevSort = prevSort,
            PageNumber = page ?? 1,
            PageSize = 25
        };

        var pagedResult = await _bookService.GetAdminBooksAsync(filter);

        var pagedList = pagedResult.ToPagedList(b => new BookListItemViewModel
        {
            Id = b.Id,
            Name = b.Name,
            Format = b.Format,
            Price = b.Price,
            QuantityInStock = b.QuantityInStock,
            IsDeleted = b.IsDeleted,
            IsVisible = b.IsVisible,
            AuthorName = b.AuthorName,
            CategoryName = b.CategoryName,
            UpdatedDate = b.UpdatedDate,
            CreatedDate = b.CreatedDate
        });

        return View(pagedList);
    }

    [AllowAnonymous]
    public async Task<ActionResult> Details(Guid id)
    {
        var book = await _bookService.GetBookDetailsAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && (!book.IsVisible || book.IsDeleted))
        {
            return NotFound();
        }

        var viewModel = new BookDetailsViewModel
        {
            Id = book.Id,
            Name = book.Name,
            Format = book.Format,
            ISBN = book.ISBN,
            Price = book.Price,
            ImgPath = book.ImgPath,
            AuthorId = book.AuthorId,
            AuthorName = book.AuthorName,
            CategoryId = book.CategoryId,
            CategoryName = book.CategoryName,
            BookStar = book.BookStar,
            PublicationDate = book.PublicationDate,
            Description = book.Description,
            QuantityInStock = book.QuantityInStock,
            IsVisible = book.IsVisible
        };

        return View(viewModel);
    }

    public async Task<ActionResult> Create()
    {
        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(forAdmin: true), "Id", "Name");
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(forAdmin: true), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(CreateBookViewModel book)
    {
        if (ModelState.IsValid)
        {
            var dto = new CreateBookDto
            {
                Name = book.Name,
                Description = book.Description,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                BookStar = book.BookStar,
                ISBN = book.ISBN,
                Format = book.Format,
                Price = book.Price,
                PublicationDate = book.PublicationDate,
                QuantityInStock = book.QuantityInStock,
                CoverImage = book.BookCoverFile.ToFileUploadDto()
            };

            var result = await _bookService.CreateBookAsync(dto);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(forAdmin: true), "Id", "Name", book.AuthorId);
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(forAdmin: true), "Id", "Name", book.CategoryId);
        return View(book);
    }

    public async Task<ActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }

        var book = await _bookService.GetBookForEditAsync(id.Value);
        if (book == null)
        {
            return NotFound();
        }

        var viewModel = new EditBookViewModel
        {
            Id = book.Id,
            AuthorId = book.AuthorId,
            CategoryId = book.CategoryId,
            BookStar = book.BookStar,
            Description = book.Description,
            Format = book.Format,
            ISBN = book.ISBN,
            QuantityInStock = book.QuantityInStock,
            PublicationDate = book.PublicationDate,
            Name = book.Name,
            Price = book.Price,
            IsVisible = book.IsVisible,
            ImgPath = book.ImgPath
        };

        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(forAdmin: true), "Id", "Name", book.AuthorId);
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(forAdmin: true), "Id", "Name", book.CategoryId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(EditBookViewModel editedBook)
    {
        if (ModelState.IsValid)
        {
            var dto = new UpdateBookDto
            {
                Id = editedBook.Id,
                Name = editedBook.Name,
                Format = editedBook.Format,
                ISBN = editedBook.ISBN,
                Price = editedBook.Price,
                IsVisible = editedBook.IsVisible,
                PublicationDate = editedBook.PublicationDate,
                QuantityInStock = editedBook.QuantityInStock,
                AuthorId = editedBook.AuthorId,
                CategoryId = editedBook.CategoryId,
                BookStar = editedBook.BookStar,
                Description = editedBook.Description,
                CoverImage = editedBook.BookCoverFile.ToFileUploadDto()
            };

            var result = await _bookService.UpdateBookAsync(dto);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                if (error.Contains("name", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Name", error);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
        }

        var book = await _bookService.GetBookForEditAsync(editedBook.Id);
        if (book != null)
        {
            editedBook.ImgPath = book.ImgPath;
        }
        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(forAdmin: true), "Id", "Name", editedBook.AuthorId);
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(forAdmin: true), "Id", "Name", editedBook.CategoryId);
        return View(editedBook);
    }

    public async Task<ActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }

        var book = await _bookService.GetBookDetailsAsync(id.Value);
        if (book == null || book.IsDeleted)
        {
            return NotFound();
        }

        var viewModel = new DeleteBookViewModel
        {
            Id = book.Id,
            Name = book.Name,
            ISBN = book.ISBN,
            AuthorName = book.AuthorName,
            ImgPath = book.ImgPath
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _bookService.DeleteBookAsync(id);
        if (!result.Succeeded)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        var result = await _bookService.RestoreBookAsync(id);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(", ", result.Errors);
        }
        else
        {
            TempData["SuccessMessage"] = "Book and its associated parent entities were successfully restored.";
        }

        return RedirectToAction(nameof(Index), new { status = "deleted" });
    }
}
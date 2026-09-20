using System.Net;
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
using AprilBookStore.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace AprilBookStore.Web.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;
    private readonly IFileStorageService _fileStorageService;

    public BooksController(
        IBookService bookService,
        IAuthorService authorService,
        ICategoryService categoryService,
        IFileStorageService fileStorageService)
    {
        _bookService = bookService;
        _authorService = authorService;
        _categoryService = categoryService;
        _fileStorageService = fileStorageService;
    }

    [AcceptVerbs("GET", "POST")]
    [AllowAnonymous]
    public async Task<IActionResult> AutoComplete(string term)
    {
        var result = (await _bookService.SearchBooksAsync(term)).Select(b => b.Name).ToList();
        return Json(result);
    }

    public async Task<IActionResult> Name(string name)
    {
        bool isUnique = await _bookService.IsBookNameUniqueAsync(name);
        return Json(isUnique);
    }

    public ActionResult Index(int? page)
    {
        var books = _bookService.GetBooks().OrderBy(b => b.Name);
        var pagedBooks = books.ToPagedList(page ?? 1, 25);
        var viewModels = pagedBooks.Select(b => new BookListItemViewModel
        {
            Id = b.Id,
            Name = b.Name,
            Format = b.Format,
            Price = b.Price,
            QuantityInStock = b.QuantityInStock,
            IsDeleted = b.IsDeleted,
            IsVisible = b.IsVisible,
            AuthorName = b.Author?.Name ?? "Unknown",
            CategoryName = b.Category?.Name ?? "Unknown"
        });
        var pagedList = new StaticPagedList<BookListItemViewModel>(viewModels, pagedBooks.GetMetaData());
        return View(pagedList);
    }

    [HttpPost]
    public async Task<ActionResult> Index(string name, Guid? categoryId, decimal? minPrice, decimal? maxPrice, int? page)
    {
        var books = await _bookService.FilterBooksAsync(name, categoryId, minPrice, maxPrice);

        ViewData["NameFilter"] = name;
        ViewData["CategoryFilter"] = categoryId;
        ViewData["MinPrice"] = minPrice;
        ViewData["MaxPrice"] = maxPrice;

        ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name");

        int pageSize = 25;
        int pageNumber = page ?? 1;
        var pagedBooks = await books.ToPagedListAsync(pageNumber, pageSize);
        var viewModels = pagedBooks.Select(b => new BookListItemViewModel
        {
            Id = b.Id,
            Name = b.Name,
            Format = b.Format,
            Price = b.Price,
            QuantityInStock = b.QuantityInStock,
            IsDeleted = b.IsDeleted,
            IsVisible = b.IsVisible,
            AuthorName = b.Author?.Name ?? "Unknown",
            CategoryName = b.Category?.Name ?? "Unknown"
        });
        var pagedList = new StaticPagedList<BookListItemViewModel>(viewModels, pagedBooks.GetMetaData());
        return View(pagedList);
    }

    [AllowAnonymous]
    public async Task<ActionResult> Details(Guid id)
    {
        Book? book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return View("NotFound");
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
            AuthorName = book.Author?.Name ?? "Unknown",
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? "Unknown",
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
        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(), "Id", "Name");
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(CreateBookViewModel book)
    {
        if (ModelState.IsValid)
        {
            var newBook = new Book
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
                IsVisible = true
            };

            if (book.BookCoverFile != null && book.BookCoverFile.Length > 0)
            {
                newBook.ImgPath = await _fileStorageService.SaveBookCoverAsync(
                    book.BookCoverFile.OpenReadStream(),
                    book.BookCoverFile.FileName);
            }

            await _bookService.CreateBookAsync(newBook);
            return RedirectToAction("Index");
        }

        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(), "Id", "Name", book.AuthorId);
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", book.CategoryId);
        return View(book);
    }

    public async Task<ActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }

        Book? book = await _bookService.GetBookByIdAsync(id.Value);
        if (book == null)
        {
            return NotFound();
        }

        var viewModel = new EditBookViewModel
        {
            Id = id.Value,
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
            IsVisible = book.IsVisible
        };

        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(), "Id", "Name", book.AuthorId);
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", book.CategoryId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(EditBookViewModel editedBook)
    {
        var book = await _bookService.GetBookByIdAsync(editedBook.Id);
        if (book == null)
        {
            return NotFound();
        }

        if (await _bookService.IsBookNameUniqueAsync(editedBook.Name, editedBook.Id))
        {
            book.Name = editedBook.Name;
            book.Format = editedBook.Format;
            book.ISBN = editedBook.ISBN;
            book.Price = editedBook.Price;
            book.IsVisible = editedBook.IsVisible;
            book.PublicationDate = editedBook.PublicationDate;
            book.QuantityInStock = editedBook.QuantityInStock;
            book.AuthorId = editedBook.AuthorId;
            book.CategoryId = editedBook.CategoryId;
            book.BookStar = editedBook.BookStar;

            if (editedBook.BookCoverFile != null && editedBook.BookCoverFile.Length > 0)
            {
                _fileStorageService.DeleteBookCover(book.ImgPath);
                book.ImgPath = await _fileStorageService.SaveBookCoverAsync(
                    editedBook.BookCoverFile.OpenReadStream(),
                    editedBook.BookCoverFile.FileName);
            }

            if (ModelState.IsValid)
            {
                await _bookService.UpdateBookAsync(book);
                return RedirectToAction("Index");
            }
        }
        else
        {
            ModelState.AddModelError("Name", "This name is used !!");
        }

        ViewBag.AuthorId = new SelectList(await _authorService.GetAuthorsAsync(), "Id", "Name", book.AuthorId);
        ViewBag.CategoryId = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", book.CategoryId);
        return View(editedBook);
    }

    public async Task<ActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return new StatusCodeResult((int)HttpStatusCode.BadRequest);
        }

        Book? book = await _bookService.GetBookByIdAsync(id.Value);
        if (book == null)
        {
            return NotFound();
        }

        var viewModel = new DeleteBookViewModel
        {
            Id = book.Id,
            Name = book.Name,
            ISBN = book.ISBN,
            AuthorName = book.Author?.Name ?? "Unknown",
            ImgPath = book.ImgPath
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(Guid id)
    {
        Book? book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _fileStorageService.DeleteBookCover(book.ImgPath);
        await _bookService.DeleteBookAsync(book.Id);

        return RedirectToAction("Index");
    }
}
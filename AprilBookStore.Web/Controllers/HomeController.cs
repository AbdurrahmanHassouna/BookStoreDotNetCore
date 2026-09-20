using AprilBookStore.Application.Interfaces;
using AprilBookStore.Web.ViewModels.Book;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace AprilBookStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _bookService;

    public HomeController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page, string? search = null)
    {
        var books = _bookService.GetBooksQuery(search)
            .Select(b => new BookCatalogItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Format = b.Format,
                Price = b.Price,
                ImgPath = b.ImgPath,
                AuthorId = b.AuthorId,
                AuthorName = b.Author != null ? b.Author.Name : string.Empty,
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : string.Empty,
                BookStar = b.BookStar,
                QuantityInStock = b.QuantityInStock
            });
        return View(await books.ToPagedListAsync(page ?? 1, 25));
    }

    [HttpPost]
    public async Task<IActionResult> Index(string search, int? page)
    {
        var books = _bookService.GetBooksQuery(search)
            .Select(b => new BookCatalogItemViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Format = b.Format,
                Price = b.Price,
                ImgPath = b.ImgPath,
                AuthorId = b.AuthorId,
                AuthorName = b.Author != null ? b.Author.Name : string.Empty,
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : string.Empty,
                BookStar = b.BookStar,
                QuantityInStock = b.QuantityInStock
            });
        return View(await books.ToPagedListAsync(page ?? 1, 25));
    }
}


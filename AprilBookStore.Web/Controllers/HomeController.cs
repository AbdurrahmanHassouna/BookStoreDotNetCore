using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Book;
using AprilBookStore.Web.Extensions;
using AprilBookStore.Web.ViewModels.Book;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _bookService;

    public HomeController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> AutoComplete(string term)
    {
        var result = await _bookService.SearchBooksAsync(term);
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page, string? search = null)
    {
        var filter = new BookCatalogFilterDto
        {
            Search = search,
            PageNumber = page ?? 1,
            PageSize = 25
        };

        var pagedResult = await _bookService.GetCatalogBooksAsync(filter);

        var pagedList = pagedResult.ToPagedList(b => new BookCatalogItemViewModel
        {
            Id = b.Id,
            Name = b.Name,
            Format = b.Format,
            Price = b.Price,
            ImgPath = b.ImgPath,
            AuthorId = b.AuthorId,
            AuthorName = b.AuthorName,
            CategoryId = b.CategoryId,
            CategoryName = b.CategoryName,
            BookStar = b.BookStar,
            QuantityInStock = b.QuantityInStock
        });

        return View(pagedList);
    }

    [HttpPost]
    public IActionResult Index(string? search, int? page)
    {
        return RedirectToAction(nameof(Index), new { search, page });
    }
}

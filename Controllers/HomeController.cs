using AprilBookStore.DataAccess;
using AprilBookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AprilBookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IData _data;
        public HomeController(IData data)
        {
            _data = data;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page, string? search = null)
        {
            var books = _data.GetBooksQuery()
                .Where(b => b.IsVisible && !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                books = books.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%"))
                             .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
                             .ThenBy(b => b.Name);
            }
            else
            {
                books = books.OrderBy(b => b.Name);
            }

            return View(await books.ToPagedListAsync(page ?? 1, 25));
        }

        [HttpPost]
        public async Task<IActionResult> Index(string search, int? page)
        {
            var books = _data.GetBooksQuery()
                .Where(b => b.IsVisible && !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                books = books.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%"))
                             .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
                             .ThenBy(b => b.Name);
            }
            else
            {
                books = books.OrderBy(b => b.Name);
            }

            return View(await books.ToPagedListAsync(page ?? 1, 25));
        }
    }
}

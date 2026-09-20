using AprilBookStore.DataAccess;
using AprilBookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AuthorsController : Controller
    {
        private readonly IData _data;

        public AuthorsController(IData data)
        {
            _data = data;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var authors = _data.GetAuthors();
            return View(authors);
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var author = _data.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,IsVisible")] Author author)
        {
            if (ModelState.IsValid)
            {
                author.CreatedDate = DateTime.UtcNow;
                author.UpdatedDate = DateTime.UtcNow;
                _data.AddAuthor(author);
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        public IActionResult Edit(int id)
        {
            var author = _data.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,IsVisible,CreatedDate")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                author.UpdatedDate = DateTime.UtcNow;
                _data.UpdateAuthor(author);
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        public IActionResult Delete(int id)
        {
            var author = _data.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var author = _data.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }

            if (author.Books != null && author.Books.Any())
            {
                ModelState.AddModelError("", "Cannot delete this author because they have associated books in the store. Please reassign or delete their books first.");
                return View("Delete", author);
            }

            _data.DeleteAuthor(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

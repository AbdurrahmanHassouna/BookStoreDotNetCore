using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Services;
    public class BookService : IBookService
    {
        private readonly IApplicationDbContext _context;

        public BookService(IApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Book> GetBooks()
        {
            return _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToList();
        }

        public IQueryable<Book> GetBooksQuery(string? search = null)
        {
            var query = _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.IsVisible && !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                query = query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%")
                                      || (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%"))
                                      || (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")))
                             .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
                             .ThenBy(b => b.Name);
            }
            else
            {
                query = query.OrderBy(b => b.Name);
            }

            return query;
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public Book? GetBookById(Guid id)
        {
            return _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefault(b => b.Id == id);
        }

        public async Task<ICollection<Book>> SearchBooksAsync(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return new List<Book>();
            }

            var searchTerm = search.Trim();
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%"))
                .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
                .ToListAsync();
        }

        public async Task<ICollection<Book>> FilterBooksAsync(string? name, Guid? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchTerm = name.Trim();
                query = query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%"));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
            }

            return await query.OrderBy(b => b.Name).ToListAsync();
        }

        public async Task<bool> IsBookNameUniqueAsync(string name, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return true;

            var query = _context.Books.Where(b => b.Name == name.Trim());
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }


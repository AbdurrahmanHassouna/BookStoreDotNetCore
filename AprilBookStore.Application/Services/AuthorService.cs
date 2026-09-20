using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Services;
    public class AuthorService : IAuthorService
    {
        private readonly IApplicationDbContext _context;

        public AuthorService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Author>> GetAuthorsAsync()
        {
            return await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public ICollection<Author> GetAuthors()
        {
            return _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                .OrderBy(a => a.Name)
                .ToList();
        }

        public async Task<Author?> GetAuthorByIdAsync(Guid id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                    .ThenInclude(b => b.Category)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public Author? GetAuthorById(Guid id)
        {
            return _context.Authors
                .Include(a => a.Books)
                    .ThenInclude(b => b.Category)
                .FirstOrDefault(a => a.Id == id);
        }

        public async Task CreateAuthorAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuthorAsync(Guid id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author != null)
            {
                if (author.Books != null && author.Books.Any())
                {
                    throw new InvalidOperationException("Cannot delete this author because they have associated books in the store. Please reassign or delete their books first.");
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }
    }


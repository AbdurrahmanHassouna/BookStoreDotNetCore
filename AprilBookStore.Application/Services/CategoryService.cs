using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Services;
    public class CategoryService : ICategoryService
    {
        private readonly IApplicationDbContext _context;

        public CategoryService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(c => c.Books)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories
                .AsNoTracking()
                .Include(c => c.Books)
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Books)
                    .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Category? GetCategoryById(Guid id)
        {
            return _context.Categories
                .Include(c => c.Books)
                    .ThenInclude(b => b.Author)
                .FirstOrDefault(c => c.Id == id);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }


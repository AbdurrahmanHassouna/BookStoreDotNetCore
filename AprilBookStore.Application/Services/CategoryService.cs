using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Category;
using AprilBookStore.Application.Models.Common;
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

    public async Task<ICollection<CategoryDto>> GetCategoriesAsync(bool forAdmin = false)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (!forAdmin)
        {
            query = query.Where(c => c.IsVisible);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsVisible = c.IsVisible,
                BookCount = c.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible)),
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate
            })
            .ToListAsync();
    }

    public async Task<CategoryDetailsDto?> GetCategoryDetailsAsync(Guid id, bool forAdmin = false)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id && !c.IsDeleted);

        if (!forAdmin)
        {
            query = query.Where(c => c.IsVisible);
        }

        return await query
            .Select(c => new CategoryDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                IsVisible = c.IsVisible,
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate,
                BookCount = c.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible)),
                Books = c.Books
                    .Where(b => !b.IsDeleted && (forAdmin || b.IsVisible))
                    .Select(b => new CategoryBookItemDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Price = b.Price,
                        ImgPath = b.ImgPath,
                        AuthorName = b.Author != null ? b.Author.Name : "Unknown Author",
                        BookStar = b.BookStar,
                        QuantityInStock = b.QuantityInStock,
                        IsVisible = b.IsVisible
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id && !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsVisible = c.IsVisible,
                BookCount = c.Books.Count(b => !b.IsDeleted),
                CreatedDate = c.CreatedDate,
                UpdatedDate = c.UpdatedDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult> CheckCategoryNameAvailabilityAsync(string name, Guid? currentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult.Failed("Category name is required.");
        }

        var trimmedLowerName = name.Trim().ToLowerInvariant();
        var existing = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name.ToLower() == trimmedLowerName && (currentCategoryId == null || c.Id != currentCategoryId.Value));

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                return ServiceResult.Failed($"A category with the name '{name.Trim()}' already exists in deleted categories. You can restore it instead of creating a duplicate.");
            }
            return ServiceResult.Failed("A category with this name already exists.");
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var availability = await CheckCategoryNameAvailabilityAsync(dto.Name);
        if (!availability.Succeeded)
        {
            return availability;
        }

        var category = new Category
        {
            Name = dto.Name.Trim(),
            IsVisible = dto.IsVisible
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.Id && !c.IsDeleted);
        if (category == null)
        {
            return ServiceResult.Failed("Category not found.");
        }

        var availability = await CheckCategoryNameAvailabilityAsync(dto.Name, dto.Id);
        if (!availability.Succeeded)
        {
            return availability;
        }

        category.Name = dto.Name.Trim();
        category.IsVisible = dto.IsVisible;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
        {
            return ServiceResult.Failed("Category not found.");
        }

        if (await HasAssociatedBooksAsync(id))
        {
            return ServiceResult.Failed("Cannot delete this category because it has associated books in the store.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return ServiceResult.Failed("Category not found.");
        }

        if (!category.IsDeleted)
        {
            return ServiceResult.Failed("Category is not deleted.");
        }

        category.IsDeleted = false;
        category.DeletedDate = null;
        category.IsVisible = true;
        category.UpdatedDate = DateTime.UtcNow;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<bool> IsCategoryNameUniqueAsync(string name, Guid? currentCategoryId = null)
    {
        var result = await CheckCategoryNameAvailabilityAsync(name, currentCategoryId);
        return result.Succeeded;
    }

    public async Task<bool> HasAssociatedBooksAsync(Guid id)
    {
        return await _context.Books
            .AnyAsync(b => b.CategoryId == id && !b.IsDeleted);
    }
}

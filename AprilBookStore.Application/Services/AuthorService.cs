using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Author;
using AprilBookStore.Application.Models.Common;
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

    public async Task<ICollection<AuthorDto>> GetAuthorsAsync(bool forAdmin = false)
    {
        var query = _context.Authors
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (!forAdmin)
        {
            query = query.Where(a => a.IsVisible && a.Books.Any(b => !b.IsDeleted && b.IsVisible));
        }

        return await query
            .OrderBy(a => a.Name)
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                IsVisible = a.IsVisible,
                BookCount = a.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible)),
                CreatedDate = a.CreatedDate,
                UpdatedDate = a.UpdatedDate
            })
            .ToListAsync();
    }

    public async Task<AuthorDetailsDto?> GetAuthorDetailsAsync(Guid id, bool forAdmin = false)
    {
        var query = _context.Authors
            .AsNoTracking()
            .Where(a => a.Id == id && !a.IsDeleted);

        if (!forAdmin)
        {
            query = query.Where(a => a.IsVisible);
        }

        return await query
            .Select(a => new AuthorDetailsDto
            {
                Id = a.Id,
                Name = a.Name,
                IsVisible = a.IsVisible,
                CreatedDate = a.CreatedDate,
                UpdatedDate = a.UpdatedDate,
                BookCount = a.Books.Count(b => !b.IsDeleted && (forAdmin || b.IsVisible)),
                Books = a.Books
                    .Where(b => !b.IsDeleted && (forAdmin || b.IsVisible))
                    .Select(b => new AuthorBookItemDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Price = b.Price,
                        ImgPath = b.ImgPath,
                        CategoryName = b.Category != null ? b.Category.Name : "General",
                        BookStar = b.BookStar,
                        QuantityInStock = b.QuantityInStock,
                        IsVisible = b.IsVisible
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AuthorDto?> GetAuthorByIdAsync(Guid id)
    {
        return await _context.Authors
            .AsNoTracking()
            .Where(a => a.Id == id && !a.IsDeleted)
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                IsVisible = a.IsVisible,
                BookCount = a.Books.Count(b => !b.IsDeleted),
                CreatedDate = a.CreatedDate,
                UpdatedDate = a.UpdatedDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult> CheckAuthorNameAvailabilityAsync(string name, Guid? currentAuthorId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult.Failed("Author name is required.");
        }

        var trimmedLowerName = name.Trim().ToLowerInvariant();
        var existing = await _context.Authors
            .FirstOrDefaultAsync(a => a.Name.ToLower() == trimmedLowerName && (currentAuthorId == null || a.Id != currentAuthorId.Value));

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                return ServiceResult.Failed($"An author with the name '{name.Trim()}' already exists in deleted authors. You can restore them instead of creating a duplicate.");
            }
            return ServiceResult.Failed("An author with this name already exists.");
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> CreateAuthorAsync(CreateAuthorDto dto)
    {
        var availability = await CheckAuthorNameAvailabilityAsync(dto.Name);
        if (!availability.Succeeded)
        {
            return availability;
        }

        var author = new Author
        {
            Name = dto.Name.Trim(),
            IsVisible = dto.IsVisible
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAuthorAsync(UpdateAuthorDto dto)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == dto.Id && !a.IsDeleted);
        if (author == null)
        {
            return ServiceResult.Failed("Author not found.");
        }

        var availability = await CheckAuthorNameAvailabilityAsync(dto.Name, dto.Id);
        if (!availability.Succeeded)
        {
            return availability;
        }

        author.Name = dto.Name.Trim();
        author.IsVisible = dto.IsVisible;

        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAuthorAsync(Guid id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (author == null)
        {
            return ServiceResult.Failed("Author not found.");
        }

        if (await HasAssociatedBooksAsync(id))
        {
            return ServiceResult.Failed("Cannot delete this author because they have associated books in the store.");
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreAuthorAsync(Guid id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (author == null)
        {
            return ServiceResult.Failed("Author not found.");
        }

        if (!author.IsDeleted)
        {
            return ServiceResult.Failed("Author is not deleted.");
        }

        author.IsDeleted = false;
        author.DeletedDate = null;
        author.IsVisible = true;
        author.UpdatedDate = DateTime.UtcNow;

        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<bool> IsAuthorNameUniqueAsync(string name, Guid? currentAuthorId = null)
    {
        var result = await CheckAuthorNameAvailabilityAsync(name, currentAuthorId);
        return result.Succeeded;
    }

    public async Task<bool> HasAssociatedBooksAsync(Guid id)
    {
        return await _context.Books
            .AnyAsync(b => b.AuthorId == id && !b.IsDeleted);
    }
}

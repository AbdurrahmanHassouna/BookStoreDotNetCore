using AprilBookStore.Application.Interfaces;
using AprilBookStore.Application.Models.Book;
using AprilBookStore.Application.Models.Common;
using AprilBookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Application.Services;

public class BookService : IBookService
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public BookService(IApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<BookListItemDto>> GetAdminBooksAsync(BookAdminFilterDto filter)
    {
        var query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = filter.Status.ToLowerInvariant() switch
            {
                "active" => query.Where(b => !b.IsDeleted && b.IsVisible),
                "deleted" => query.Where(b => b.IsDeleted),
                "hidden" => query.Where(b => !b.IsDeleted && !b.IsVisible),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.Trim();
            query = query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%")
                                     || (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%"))
                                     || (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == filter.CategoryId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(b => b.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(b => b.Price <= filter.MaxPrice.Value);
        }

        query = ApplySorting(query, filter.SortOrder, filter.PrevSort);

        var totalCount = await query.CountAsync();
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 25 : filter.PageSize;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookListItemDto
            {
                Id = b.Id,
                Name = b.Name,
                Format = b.Format,
                Price = b.Price,
                QuantityInStock = b.QuantityInStock,
                IsDeleted = b.IsDeleted,
                IsVisible = b.IsVisible,
                AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                CategoryName = b.Category != null ? b.Category.Name : "Unknown",
                UpdatedDate = b.UpdatedDate,
                CreatedDate = b.CreatedDate
            })
            .ToListAsync();

        return new PagedResult<BookListItemDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItemCount = totalCount
        };
    }

    public async Task<PagedResult<BookCatalogItemDto>> GetCatalogBooksAsync(BookCatalogFilterDto filter)
    {
        var query = _context.Books.AsNoTracking()
            .Where(b => b.IsVisible && !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.Trim();
            query = query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%")
                                     || (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%"))
                                     || (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == filter.CategoryId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(b => b.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(b => b.Price <= filter.MaxPrice.Value);
        }

        query = ApplySorting(query, filter.SortOrder, null);

        var totalCount = await query.CountAsync();
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 25 : filter.PageSize;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookCatalogItemDto
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
            })
            .ToListAsync();

        return new PagedResult<BookCatalogItemDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItemCount = totalCount
        };
    }

    public async Task<BookDetailsDto?> GetBookDetailsAsync(Guid id)
    {
        return await _context.Books.AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDetailsDto
            {
                Id = b.Id,
                Name = b.Name,
                Format = b.Format,
                ISBN = b.ISBN,
                Price = b.Price,
                ImgPath = b.ImgPath,
                AuthorId = b.AuthorId,
                AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : "Unknown",
                BookStar = b.BookStar,
                PublicationDate = b.PublicationDate,
                Description = b.Description,
                QuantityInStock = b.QuantityInStock,
                IsVisible = b.IsVisible,
                IsDeleted = b.IsDeleted
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EditBookDto?> GetBookForEditAsync(Guid id)
    {
        return await _context.Books.AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new EditBookDto
            {
                Id = b.Id,
                Name = b.Name,
                Format = b.Format,
                ISBN = b.ISBN,
                Price = b.Price,
                CategoryId = b.CategoryId,
                AuthorId = b.AuthorId,
                BookStar = b.BookStar,
                PublicationDate = b.PublicationDate,
                Description = b.Description,
                QuantityInStock = b.QuantityInStock,
                ImgPath = b.ImgPath,
                IsVisible = b.IsVisible
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ICollection<string>> SearchBooksAsync(string search, bool forAdmin = false)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return new List<string>();
        }

        var query = _context.Books.AsNoTracking();
        if (!forAdmin)
        {
            query = query.Where(b => b.IsVisible && !b.IsDeleted);
        }

        var searchTerm = search.Trim();
        var result = await query.Where(b => EF.Functions.Like(b.Name, $"%{searchTerm}%")
                                      || (b.Author != null && EF.Functions.Like(b.Author.Name, $"%{searchTerm}%"))
                                      || (b.Category != null && EF.Functions.Like(b.Category.Name, $"%{searchTerm}%")))
            .OrderBy(b => !EF.Functions.Like(b.Name, $"{searchTerm}%"))
            .ThenBy(b => b.Name)
            .Select(b => b.Name)
            .ToListAsync();

        return result;
    }

    public async Task<ServiceResult> CheckBookNameAvailabilityAsync(string name, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return ServiceResult.Success();

        var trimmedLowerName = name.Trim().ToLowerInvariant();
        var existing = await _context.Books
            .FirstOrDefaultAsync(b => b.Name.ToLower() == trimmedLowerName && (excludeId == null || b.Id != excludeId.Value));

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                return ServiceResult.Failed($"A book with the name '{name.Trim()}' already exists in deleted books. You can restore it instead of creating a duplicate.");
            }
            return ServiceResult.Failed("A book with this name already exists.");
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> CheckBookISBNAvailabilityAsync(string isbn, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return ServiceResult.Success();

        var trimmedLowerIsbn = isbn.Trim().ToLowerInvariant();
        var existing = await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN.ToLower() == trimmedLowerIsbn && (excludeId == null || b.Id != excludeId.Value));

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                return ServiceResult.Failed($"A book with ISBN '{isbn.Trim()}' already exists in deleted books ('{existing.Name}'). You can restore it instead of creating a duplicate.");
            }
            return ServiceResult.Failed($"A book with ISBN '{isbn.Trim()}' already exists ('{existing.Name}').");
        }

        return ServiceResult.Success();
    }

    public async Task<bool> IsBookNameUniqueAsync(string name, Guid? excludeId = null)
    {
        var result = await CheckBookNameAvailabilityAsync(name, excludeId);
        return result.Succeeded;
    }

    public async Task<ServiceResult<Guid>> CreateBookAsync(CreateBookDto dto)
    {
        var nameCheck = await CheckBookNameAvailabilityAsync(dto.Name);
        if (!nameCheck.Succeeded)
        {
            return ServiceResult<Guid>.Failed(nameCheck.Errors);
        }

        var isbnCheck = await CheckBookISBNAvailabilityAsync(dto.ISBN);
        if (!isbnCheck.Succeeded)
        {
            return ServiceResult<Guid>.Failed(isbnCheck.Errors);
        }

        string? imgPath = null;
        if (dto.CoverImage != null && dto.CoverImage.Length > 0)
        {
            try
            {
                imgPath = await _fileStorageService.SaveBookCoverAsync(
                    dto.CoverImage.Content,
                    dto.CoverImage.FileName);
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<Guid>.Failed(ex.Message);
            }
        }

        var book = new Book
        {
            Name = dto.Name,
            Description = dto.Description,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId,
            BookStar = dto.BookStar,
            ISBN = dto.ISBN,
            Format = dto.Format,
            Price = dto.Price,
            PublicationDate = dto.PublicationDate,
            QuantityInStock = dto.QuantityInStock,
            IsVisible = true,
            ImgPath = imgPath
        };

        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == dto.AuthorId);
        if (author != null && !author.IsVisible)
        {
            author.IsVisible = true;
            author.UpdatedDate = DateTime.UtcNow;
        }

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
        if (category != null && !category.IsVisible)
        {
            category.IsVisible = true;
            category.UpdatedDate = DateTime.UtcNow;
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return ServiceResult<Guid>.Success(book.Id);
    }

    public async Task<ServiceResult> UpdateBookAsync(UpdateBookDto dto)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == dto.Id);
        if (book == null)
        {
            return ServiceResult.Failed("Book not found.");
        }

        var nameCheck = await CheckBookNameAvailabilityAsync(dto.Name, dto.Id);
        if (!nameCheck.Succeeded)
        {
            return nameCheck;
        }

        var isbnCheck = await CheckBookISBNAvailabilityAsync(dto.ISBN, dto.Id);
        if (!isbnCheck.Succeeded)
        {
            return isbnCheck;
        }

        bool visibilityChanged = book.IsVisible != dto.IsVisible;

        book.Name = dto.Name;
        book.Format = dto.Format;
        book.ISBN = dto.ISBN;
        book.Price = dto.Price;
        book.IsVisible = dto.IsVisible;
        book.PublicationDate = dto.PublicationDate;
        book.QuantityInStock = dto.QuantityInStock;
        book.AuthorId = dto.AuthorId;
        book.CategoryId = dto.CategoryId;
        book.BookStar = dto.BookStar;
        book.Description = dto.Description;

        string? oldCover = null;
        if (dto.CoverImage != null && dto.CoverImage.Length > 0)
        {
            try
            {
                oldCover = book.ImgPath;
                book.ImgPath = await _fileStorageService.SaveBookCoverAsync(
                    dto.CoverImage.Content,
                    dto.CoverImage.FileName);
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult.Failed(ex.Message);
            }
        }

        if (visibilityChanged)
        {
            if (!dto.IsVisible)
            {
                bool authorHasOtherBooks = await _context.Books
                    .AnyAsync(b => b.AuthorId == book.AuthorId && b.Id != book.Id && !b.IsDeleted && b.IsVisible);
                if (!authorHasOtherBooks && book.Author != null)
                {
                    book.Author.IsVisible = false;
                    book.Author.UpdatedDate = DateTime.UtcNow;
                }

                bool categoryHasOtherBooks = await _context.Books
                    .AnyAsync(b => b.CategoryId == book.CategoryId && b.Id != book.Id && !b.IsDeleted && b.IsVisible);
                if (!categoryHasOtherBooks && book.Category != null)
                {
                    book.Category.IsVisible = false;
                    book.Category.UpdatedDate = DateTime.UtcNow;
                }
            }
            else
            {
                if (book.Author != null && !book.Author.IsVisible)
                {
                    book.Author.IsVisible = true;
                    book.Author.UpdatedDate = DateTime.UtcNow;
                }
                if (book.Category != null && !book.Category.IsVisible)
                {
                    book.Category.IsVisible = true;
                    book.Category.UpdatedDate = DateTime.UtcNow;
                }
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult.Failed("This book was modified by another process. Please reload the page and try again.");
        }

        if (oldCover != null)
        {
            _fileStorageService.DeleteBookCover(oldCover);
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteBookAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (book == null)
        {
            return ServiceResult.Failed("Book not found.");
        }

        _context.Books.Remove(book);

        bool authorHasOtherBooks = await _context.Books
            .AnyAsync(b => b.AuthorId == book.AuthorId && b.Id != book.Id && !b.IsDeleted && b.IsVisible);
        if (!authorHasOtherBooks && book.Author != null)
        {
            book.Author.IsVisible = false;
            book.Author.UpdatedDate = DateTime.UtcNow;
        }

        bool categoryHasOtherBooks = await _context.Books
            .AnyAsync(b => b.CategoryId == book.CategoryId && b.Id != book.Id && !b.IsDeleted && b.IsVisible);
        if (!categoryHasOtherBooks && book.Category != null)
        {
            book.Category.IsVisible = false;
            book.Category.UpdatedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreBookAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return ServiceResult.Failed("Book not found.");
        }

        if (!book.IsDeleted)
        {
            return ServiceResult.Failed("Book is not deleted.");
        }

        book.IsDeleted = false;
        book.DeletedDate = null;
        book.UpdatedDate = DateTime.UtcNow;

        if (book.Author != null)
        {
            if (book.Author.IsDeleted)
            {
                book.Author.IsDeleted = false;
                book.Author.DeletedDate = null;
            }
            book.Author.IsVisible = true;
            book.Author.UpdatedDate = DateTime.UtcNow;
            _context.Authors.Update(book.Author);
        }

        if (book.Category != null)
        {
            if (book.Category.IsDeleted)
            {
                book.Category.IsDeleted = false;
                book.Category.DeletedDate = null;
            }
            book.Category.IsVisible = true;
            book.Category.UpdatedDate = DateTime.UtcNow;
            _context.Categories.Update(book.Category);
        }

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    private static IQueryable<Book> ApplySorting(IQueryable<Book> query, string? sortOrder, string? prevSort)
    {
        var primary = sortOrder?.ToLowerInvariant() ?? "date_desc";

        IOrderedQueryable<Book> orderedQuery = primary switch
        {
            "name_asc" => query.OrderBy(b => b.Name),
            "name_desc" => query.OrderByDescending(b => b.Name),
            "author_asc" => query.OrderBy(b => b.Author != null ? b.Author.Name : string.Empty),
            "author_desc" => query.OrderByDescending(b => b.Author != null ? b.Author.Name : string.Empty),
            "category_asc" => query.OrderBy(b => b.Category != null ? b.Category.Name : string.Empty),
            "category_desc" => query.OrderByDescending(b => b.Category != null ? b.Category.Name : string.Empty),
            "price_asc" => query.OrderBy(b => b.Price),
            "price_desc" => query.OrderByDescending(b => b.Price),
            "qty_asc" => query.OrderBy(b => b.QuantityInStock),
            "qty_desc" => query.OrderByDescending(b => b.QuantityInStock),
            "date_asc" => query.OrderBy(b => b.UpdatedDate),
            "date_desc" => query.OrderByDescending(b => b.UpdatedDate),
            _ => query.OrderByDescending(b => b.UpdatedDate)
        };

        var secondary = prevSort?.ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(secondary))
        {
            var primaryCol = primary.Split('_')[0];
            var secondaryCol = secondary.Split('_')[0];

            if (!string.Equals(primaryCol, secondaryCol, StringComparison.OrdinalIgnoreCase))
            {
                orderedQuery = secondary switch
                {
                    "name_asc" => orderedQuery.ThenBy(b => b.Name),
                    "name_desc" => orderedQuery.ThenByDescending(b => b.Name),
                    "author_asc" => orderedQuery.ThenBy(b => b.Author != null ? b.Author.Name : string.Empty),
                    "author_desc" => orderedQuery.ThenByDescending(b => b.Author != null ? b.Author.Name : string.Empty),
                    "category_asc" => orderedQuery.ThenBy(b => b.Category != null ? b.Category.Name : string.Empty),
                    "category_desc" => orderedQuery.ThenByDescending(b => b.Category != null ? b.Category.Name : string.Empty),
                    "price_asc" => orderedQuery.ThenBy(b => b.Price),
                    "price_desc" => orderedQuery.ThenByDescending(b => b.Price),
                    "qty_asc" => orderedQuery.ThenBy(b => b.QuantityInStock),
                    "qty_desc" => orderedQuery.ThenByDescending(b => b.QuantityInStock),
                    "date_asc" => orderedQuery.ThenBy(b => b.UpdatedDate),
                    "date_desc" => orderedQuery.ThenByDescending(b => b.UpdatedDate),
                    _ => orderedQuery
                };
            }
        }

        return orderedQuery.ThenBy(b => b.Id);
    }
}
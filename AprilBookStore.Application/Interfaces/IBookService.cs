using AprilBookStore.Application.Models.Book;
using AprilBookStore.Application.Models.Common;

namespace AprilBookStore.Application.Interfaces;

public interface IBookService
{
    Task<PagedResult<BookListItemDto>> GetAdminBooksAsync(BookAdminFilterDto filter);
    Task<PagedResult<BookCatalogItemDto>> GetCatalogBooksAsync(BookCatalogFilterDto filter);
    Task<BookDetailsDto?> GetBookDetailsAsync(Guid id);
    Task<EditBookDto?> GetBookForEditAsync(Guid id);
    Task<ICollection<string>> SearchBooksAsync(string search, bool forAdmin = false);
    Task<bool> IsBookNameUniqueAsync(string name, Guid? excludeId = null);
    Task<ServiceResult> CheckBookNameAvailabilityAsync(string name, Guid? excludeId = null);
    Task<ServiceResult> CheckBookISBNAvailabilityAsync(string isbn, Guid? excludeId = null);
    Task<ServiceResult<Guid>> CreateBookAsync(CreateBookDto dto);
    Task<ServiceResult> UpdateBookAsync(UpdateBookDto dto);
    Task<ServiceResult> DeleteBookAsync(Guid id);
    Task<ServiceResult> RestoreBookAsync(Guid id);
}

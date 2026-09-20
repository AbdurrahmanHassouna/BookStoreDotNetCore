using AprilBookStore.Domain.Entities;

namespace AprilBookStore.Application.Interfaces;

public interface IBookService
{
    ICollection<Book> GetBooks();
    IQueryable<Book> GetBooksQuery(string? search = null);
    Task<Book?> GetBookByIdAsync(Guid id);
    Book? GetBookById(Guid id);
    Task<ICollection<Book>> SearchBooksAsync(string search);
    Task<ICollection<Book>> FilterBooksAsync(string? name, Guid? categoryId, decimal? minPrice, decimal? maxPrice);
    Task<bool> IsBookNameUniqueAsync(string name, Guid? excludeId = null);
    Task CreateBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(Guid id);
}

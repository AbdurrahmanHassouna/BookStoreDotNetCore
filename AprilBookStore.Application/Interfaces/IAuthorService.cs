using AprilBookStore.Domain.Entities;

namespace AprilBookStore.Application.Interfaces;
    public interface IAuthorService
    {
        Task<ICollection<Author>> GetAuthorsAsync();
        ICollection<Author> GetAuthors();
        Task<Author?> GetAuthorByIdAsync(Guid id);
        Author? GetAuthorById(Guid id);
        Task CreateAuthorAsync(Author author);
        Task UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(Guid id);
    }


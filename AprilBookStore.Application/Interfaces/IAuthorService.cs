using AprilBookStore.Application.Models.Author;
using AprilBookStore.Application.Models.Common;

namespace AprilBookStore.Application.Interfaces;

public interface IAuthorService
{
    Task<ICollection<AuthorDto>> GetAuthorsAsync(bool forAdmin = false);
    Task<AuthorDetailsDto?> GetAuthorDetailsAsync(Guid id, bool forAdmin = false);
    Task<AuthorDto?> GetAuthorByIdAsync(Guid id);
    Task<ServiceResult> CreateAuthorAsync(CreateAuthorDto dto);
    Task<ServiceResult> UpdateAuthorAsync(UpdateAuthorDto dto);
    Task<ServiceResult> DeleteAuthorAsync(Guid id);
    Task<ServiceResult> RestoreAuthorAsync(Guid id);
    Task<bool> IsAuthorNameUniqueAsync(string name, Guid? currentAuthorId = null);
    Task<bool> HasAssociatedBooksAsync(Guid id);
}

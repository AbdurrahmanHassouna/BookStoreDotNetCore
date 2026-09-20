namespace AprilBookStore.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveBookCoverAsync(Stream fileStream, string originalFileName);
    void DeleteBookCover(string? relativePath);
}

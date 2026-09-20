using AprilBookStore.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace AprilBookStore.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveBookCoverAsync(Stream fileStream, string originalFileName)
    {
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(originalFileName);
        string uploadFolder = Path.Combine(_environment.WebRootPath, "book-covers", "UploadedCovers");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        string fullPath = Path.Combine(uploadFolder, uniqueFileName);
        await using (var outputStream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream);
        }

        return "/book-covers/UploadedCovers/" + uniqueFileName;
    }

    public void DeleteBookCover(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        string fileName = Path.GetFileName(relativePath.TrimStart('/', '\\'));
        string fullPath = Path.Combine(_environment.WebRootPath, "book-covers", "UploadedCovers", fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}

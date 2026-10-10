namespace AprilBookStore.Application.Models.Common;

public record FileUploadDto(Stream Content, string FileName, long Length);

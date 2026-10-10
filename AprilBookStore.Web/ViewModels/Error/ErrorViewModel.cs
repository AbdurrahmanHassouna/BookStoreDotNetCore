namespace AprilBookStore.Web.ViewModels.Error;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public int? StatusCode { get; set; }
    public string? ErrorTitle { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RoleId { get; set; }

    public string? ExceptionPath { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }

    public bool HasCustomError => !string.IsNullOrWhiteSpace(ErrorTitle) || !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasExceptionDetails => !string.IsNullOrWhiteSpace(ExceptionMessage) || !string.IsNullOrWhiteSpace(StackTrace) || !string.IsNullOrWhiteSpace(ExceptionPath);
    public bool HasStatusCode => StatusCode.HasValue;
}

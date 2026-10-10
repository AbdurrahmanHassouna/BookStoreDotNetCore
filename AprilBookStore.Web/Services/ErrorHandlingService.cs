using AprilBookStore.Web.ViewModels.Error;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AprilBookStore.Web.Services;

public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;

    public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
    {
        _logger = logger;
    }

    public ErrorViewModel HandleStatusCode(int statusCode, HttpContext httpContext)
    {
        var statusCodeResult = httpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var originalPath = statusCodeResult?.OriginalPath;
        var originalQueryString = statusCodeResult?.OriginalQueryString;

        switch (statusCode)
        {
            case 404:
                _logger.LogWarning("404 error path: {Path}, query: {Query}", originalPath, originalQueryString);
                return new ErrorViewModel
                {
                    StatusCode = statusCode,
                    ErrorTitle = "Page Not Found",
                    ErrorMessage = "The resource you are looking for was not found.",
                    ExceptionPath = originalPath
                };
            default:
                _logger.LogWarning("HTTP {StatusCode} error path: {Path}, query: {Query}", statusCode, originalPath, originalQueryString);
                return new ErrorViewModel
                {
                    StatusCode = statusCode,
                    ErrorTitle = $"Error {statusCode}",
                    ErrorMessage = $"An error occurred with status code {statusCode}.",
                    ExceptionPath = originalPath
                };
        }
    }

    public ErrorViewModel HandleException(HttpContext httpContext)
    {
        var exceptionDetails = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        var path = exceptionDetails?.Path;
        var error = exceptionDetails?.Error;

        _logger.LogError(error, "The path {Path} threw an exception: {Message}", path, error?.Message);

        return new ErrorViewModel
        {
            StatusCode = 500,
            ErrorTitle = "An Error Occurred",
            ErrorMessage = "An unexpected error occurred while processing your request.",
            ExceptionPath = path,
            ExceptionMessage = error?.Message,
            StackTrace = error?.StackTrace
        };
    }
}

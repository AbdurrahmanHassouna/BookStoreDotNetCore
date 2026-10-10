using AprilBookStore.Web.ViewModels.Error;
using Microsoft.AspNetCore.Http;

namespace AprilBookStore.Web.Services;

public interface IErrorHandlingService
{
    ErrorViewModel HandleStatusCode(int statusCode, HttpContext httpContext);
    ErrorViewModel HandleException(HttpContext httpContext);
}

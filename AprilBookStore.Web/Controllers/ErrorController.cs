using AprilBookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AprilBookStore.Web.Controllers;

public class ErrorController : Controller
{
    private readonly IErrorHandlingService _errorHandlingService;

    public ErrorController(IErrorHandlingService errorHandlingService)
    {
        _errorHandlingService = errorHandlingService;
    }

    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var model = _errorHandlingService.HandleStatusCode(statusCode, HttpContext);
        return View("NotFound", model);
    }

    public IActionResult Error()
    {
        var model = _errorHandlingService.HandleException(HttpContext);
        return View("Error", model);
    }
}

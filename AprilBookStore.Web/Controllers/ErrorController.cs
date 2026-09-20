using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AprilBookStore.Web.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode) 
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        switch (statusCode)
        {
            case 404:
                ViewBag.ErrorMessage = "Not Found";
                ViewBag.StatusCode = statusCode;
                _logger.LogWarning($" 404 error path  {statusCodeResult?.OriginalPath} query {statusCodeResult?.OriginalQueryString}");
                break;
            default:
                ViewBag.StatusCode = statusCode;
                break;
        }

        return View("NotFound");
    }

    public IActionResult Error()
    {
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        ViewBag.ExceptionPath = exceptionDetails?.Path;
        ViewBag.ExceptionMessage = exceptionDetails?.Error.Message;
        ViewBag.StackTrace = exceptionDetails?.Error.StackTrace;
        _logger.LogError($"The path {exceptionDetails?.Path} threw an exception {exceptionDetails?.Error}");

        return View("Error");
    }
}

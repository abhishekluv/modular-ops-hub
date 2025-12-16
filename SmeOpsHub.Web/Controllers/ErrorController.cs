using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmeOpsHub.Web.Controllers;

public class ErrorController : Controller
{
    [Route("error")]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewBag.Path = feature?.Path;
        ViewBag.ErrorMessage = feature?.Error.Message;
        return View("Error");
    }

    [Route("error/{statusCode:int}")]
    public IActionResult StatusCodePage(int statusCode)
    {
        ViewBag.StatusCode = statusCode;
        return View("StatusCode");
    }
}

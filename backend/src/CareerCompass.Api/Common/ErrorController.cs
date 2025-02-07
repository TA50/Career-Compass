using System.Diagnostics;
using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Common;

public class ErrorController(ApiControllerContext context) : ApiController(context)
{
    [Route("/error")]
    [AllowAnonymous]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        var err = exceptionHandlerFeature.Error;
        var statusCode = err switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };


        return Problem(
            err.Message,
            instance: exceptionHandlerFeature.Path,
            statusCode: statusCode,
            title: err.Message
        );
    }
}
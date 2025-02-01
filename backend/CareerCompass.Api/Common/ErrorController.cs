using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Common;

public class ErrorController : ApiController
{
    [Route("/error")]
    public IActionResult HandleError()
    {
        var problemDetails = MapError(Error.Unexpected("An unexpected error occurred"));
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}
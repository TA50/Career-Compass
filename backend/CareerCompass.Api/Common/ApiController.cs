using Microsoft.AspNetCore.Mvc;
using ErrorOr;

namespace CareerCompass.Api.Common;

public class ApiController : ControllerBase
{
    protected ProblemDetails MapError(
        IList<Error> errors,
        string? title = null,
        int? statusCode = null,
        string? details = null)
    {
        if (errors.Count == 0)
        {
            return new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = details
            };
        }

        if (errors.Count == 1)
        {
            return MapError(errors.First(),
                title, statusCode, details);
        }


        var first = MapError(errors.First(),
            title,
            statusCode,
            details);

        return new ProblemDetails
        {
            Title = title ?? first.Title,
            Status = statusCode ?? first.Status,
            Detail = details ?? first.Detail,
            Extensions = new Dictionary<string, object?>
            {
                ["errors"] = errors.Select(e => e.Description)
            }
        };
    }

    protected ProblemDetails MapError(Error error,
        string? title = null,
        int? inStatusCode = null,
        string? details = null)
    {
        var statusCode = inStatusCode ?? error.Type switch
        {
            ErrorType.Conflict => 404,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.Validation => 400,
            ErrorType.Unexpected => 500,
            ErrorType.Failure => 500,
            _ => 500
        };

        var extensions = new Dictionary<string, object?>();
        var metadata = error.Metadata;
        if (metadata is not null)
        {
            foreach (var (key, value) in metadata)
            {
                extensions[key] = value;
            }
        }


        return new ProblemDetails
        {
            Title = title ?? error.Code,
            Status = statusCode,
            Detail = details ?? error.Description,
            Extensions = extensions
        };
    }
}
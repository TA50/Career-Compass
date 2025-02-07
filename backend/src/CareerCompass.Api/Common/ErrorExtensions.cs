using CareerCompass.Application.Common;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Common;

public static class ErrorExtensions
{
    public static ProblemDetails ToProblemDetails(
        this IList<Error> errors,
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

        var first = errors.First().ToProblemDetails(
            title,
            statusCode,
            details);
        if (errors.Count == 1)
        {
            return first;
        }

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

    public static IActionResult ToActionResult(this ProblemDetails problem)
    {
        return new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };
    }

    public static ActionResult<T> ToActionResult<T>(this ProblemDetails problem)
    {
        return new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };
    }

    public static ProblemDetails ToProblemDetails(
        this Error error,
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

        var extensions = new Dictionary<string, object?> { { "code", error.Code } };
        var metadata = error.Metadata;


        string problemTitle = title ?? error.Code + " - " + error.Type.ToString();

        if (metadata is not null)
        {
            foreach (var (key, value) in metadata)
            {
                extensions[key] = value;
            }

            if (metadata.TryGetValue(ErrorMetaDataKey.Title, out var titleValue))
            {
                problemTitle = titleValue.ToString() ?? problemTitle;
            }
        }


        return new ProblemDetails
        {
            Title = problemTitle,
            Status = statusCode,
            Detail = details ?? error.Description,
            Extensions = extensions
        };
    }
}
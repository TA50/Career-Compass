using System.Net;
using CareerCompass.Core.Common;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Extensions;

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

        if (first.Status == StatusCodes.Status400BadRequest)
        {
            return errors.ToValidationProblemDetails();
        }


        return first;
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
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
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


    public static ProblemDetails ToValidationProblemDetails(
        this IList<Error> errors)
    {
        var problem = new ValidationProblemDetails()
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        foreach (var error in errors)
        {
            if (problem.Errors.ContainsKey(error.Code))
            {
                var previousErrors = problem.Errors[error.Code];
                problem.Errors[error.Code] = [..previousErrors, error.Description];
                continue;
            }

            problem.Errors.Add(error.Code, [error.Description]);
        }

        return problem;
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var validationProblemDetails = new ValidationProblemDetails
{
    Title = "One or more validation errors occurred.",
    Status = StatusCodes.Status400BadRequest
};
validationProblemDetails.Errors.Add("test key", ["test error", "test error 2"]);

var json = System.Text.Json.JsonSerializer.Serialize(validationProblemDetails);
Console.WriteLine(json);
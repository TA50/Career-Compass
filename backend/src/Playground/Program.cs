using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Core.Common.Models;
using System.Text.Json;

var text = @"
{""items"":[""1"", ""2""],""totalPages"":3,""nextPage"":null,""page"":3,""previousPage"":2}
";

var result = JsonSerializer.Deserialize<PaginationResult<string>>(text, new JsonSerializerOptions()
{
    PropertyNameCaseInsensitive = true,
});
Console.WriteLine(result.Items.Count); // 3
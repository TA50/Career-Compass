using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CareerCompass.Api.OpenApi;

public class AppOpenApiDocumentTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var appUrl = configuration["API_URL"];
        if (appUrl is not null)
        {
            document.Servers.Clear();
            document.Servers.Add(
                new OpenApiServer
                {
                    Description = "Local",
                    Url = appUrl
                }
            );
        }

        return Task.CompletedTask;
    }
}
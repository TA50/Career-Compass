using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.AddContainer("MailHog", "mailhog/mailhog")
        .WithEndpoint(port: 8025, targetPort: 8025, name: "web-ui", scheme: "http")
        .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp-server");
}

builder.AddProject<CareerCompass_Api>("webapi");

builder.Build().Run();
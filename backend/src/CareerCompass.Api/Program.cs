using CareerCompass.Api;
using CareerCompass.Api.Controllers;
using CareerCompass.Aspire.ServiceDefaults;
using CareerCompass.Core;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Infrastructure;
using CareerCompass.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructure(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.BindInfrastructureConfiguration(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddApi(builder.Configuration);

#region Pipeline

var app = builder.Build();

app.UseExceptionHandler("/error");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/test", (HttpContext context) =>
    {
        var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
        var confirmUrl = linkGenerator.GetUriByName(context,
            nameof(UserController.ConfirmEmail),
            values: new { userId = Guid.NewGuid(), code = "123111", returnUrl = "http://localhost:5000" });

        return Results.Ok(new
        {
            confirmUrl
        });
    })
    .AllowAnonymous();
app.MapControllers();

#endregion

app.Run();
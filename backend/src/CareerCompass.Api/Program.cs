using CareerCompass.Api;
using CareerCompass.Aspire.ServiceDefaults;
using CareerCompass.Core;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Infrastructure;
using CareerCompass.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
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

app.MapGet("/test", async (IEmailSender emailSender) =>
    {
        await emailSender.Send("awadalbrkal@gmail.com", "Test", "Hello");
        return Results.Ok("Email Sent!");
    })
    .WithName("Test")
    .AllowAnonymous();

app.MapControllers();

#endregion

app.Run();
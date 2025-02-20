using CareerCompass.Api;
using CareerCompass.Api.Controllers;
using CareerCompass.Aspire.ServiceDefaults;
using CareerCompass.Core;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructure(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.ConfigureSettings();

builder.Services.AddCore();
builder.Services.AddApi(builder.Configuration);

builder.Services.Configure<CoreSettings>(
    builder.Configuration.GetSection(nameof(CoreSettings)));


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


app.MapGet("/test",
        async (HttpContext context, IEmailSender emailSender) =>
        {
            await emailSender.Send(new PlainTextMail("test@gmail.com", "hellow@gmail.com").WithBody("test"));
            return Results.Ok();
        })
    .AllowAnonymous();
app.MapControllers();

#endregion

app.Run();
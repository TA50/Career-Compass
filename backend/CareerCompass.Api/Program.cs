using CareerCompass.Api.Common;
using CareerCompass.Application;
using CareerCompass.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddControllers();

builder.Services.AddScoped<UserContext>();

builder.Services.AddScoped<SetUserContextMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<SetUserContextMiddleware>();

app.MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
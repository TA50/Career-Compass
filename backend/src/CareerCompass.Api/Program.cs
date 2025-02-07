using CareerCompass.Api.Common;
using CareerCompass.Application;
using CareerCompass.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddApplication();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddControllers(o => { o.Filters.Add(new AuthorizeFilter()); });
builder.Services.AddScoped<ApiControllerContext>();

builder.Services.AddScoped<UserContext>();

builder.Services.AddScoped<SetUserContextMiddleware>();

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


app.UseMiddleware<SetUserContextMiddleware>();

app.MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
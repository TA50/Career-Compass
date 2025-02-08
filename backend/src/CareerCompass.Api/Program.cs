using CareerCompass.Api.Controllers;
using CareerCompass.Core;
using CareerCompass.Infrastructure;
using CareerCompass.Infrastructure.Persistence;
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


app.MapIdentityApi<ApplicationIdentityUser>();

app.MapControllers();

app.Run();
using System.Security.Claims;
using CareerCompass.Playground;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Local")));

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<IEmailSender<User>, IdentityEmailSender>();
builder.Services.Configure<IdentityOptions>(opts => { opts.User.RequireUniqueEmail = true; });
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated(); // Use this for creating the database without migrations
    context.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");
app.MapPut("/account-details", async (UserManager<User> userManager,
    HttpContext context,
    [FromBody] UpdateAccountDetailsDto dto) =>
{
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var user = await userManager.FindByIdAsync(userId);
    if (user is null)
    {
        return Results.Unauthorized();
    }

    user.Bio = dto.Bio;
    user.DisplayName = dto.DisplayName;
    var result = await userManager.UpdateAsync(user);
    return Results.Ok(result);
}).RequireAuthorization();
app.Run();
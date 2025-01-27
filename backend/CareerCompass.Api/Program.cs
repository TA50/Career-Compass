using System.Reflection;
using CareerCompass.Application;
using CareerCompass.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using AutoMapper;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// builder.Services.AddControllers();
// builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([
    Assembly.GetAssembly(typeof(Program)) ?? throw new InvalidOperationException(),
    Assembly.GetAssembly(typeof(ApplicationAssemblyMarker)) ?? throw new InvalidOperationException()
]));


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

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

app.MapIdentityApi<IdentityUser>();
// app.MapControllers();


app.Run();
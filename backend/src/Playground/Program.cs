// See https://aka.ms/new-console-template for more information

using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>() // Use AddUserSecrets
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

var dbContext = new AppDbContext(options);

dbContext.Tags.Add(Tag.Create(UserId.CreateUnique(), "Test"));
dbContext.SaveChanges();
var tag = dbContext.Tags.FirstOrDefault();
Console.WriteLine("Hello World!");
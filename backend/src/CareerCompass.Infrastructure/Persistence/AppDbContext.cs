using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence;

internal class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureAssemblyMarker).Assembly);

        base.OnModelCreating(modelBuilder);
    }


    #region Tables

    internal DbSet<Scenario> Scenarios { get; set; }
    internal DbSet<User> Users { get; set; }
    internal DbSet<Field> Fields { get; set; }
    internal DbSet<Tag> Tags { get; set; }

    #endregion
}
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence;

internal class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationIdentityUser, IdentityRole<UserId>, UserId>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureAssemblyMarker).Assembly);

        modelBuilder.Entity<IdentityRole<UserId>>()
            .Property(r => r.Id)
            .HasConversion(id => id.Value, s => UserId.Create(s));


        base.OnModelCreating(modelBuilder);
    }


    #region Tables

    internal DbSet<Scenario> Scenarios { get; set; }
    internal new DbSet<User> Users { get; set; } // new keyword to hide IdentityDbContext.Users
    internal DbSet<ApplicationIdentityUser> IdentityUsers { get; set; }
    internal DbSet<Field> Fields { get; set; }
    internal DbSet<Tag> Tags { get; set; }

    #endregion
}
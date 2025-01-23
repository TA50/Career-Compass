using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Fields;
using CareerCompass.Infrastructure.Scenarios;
using CareerCompass.Infrastructure.Tags;
using CareerCompass.Infrastructure.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserTable>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ScenarioFieldTable>()
            .HasKey(entity => new { entity.ScenarioId, entity.FieldId });
    }


    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<IAuditable>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
            }

            entry.Entity.UpdatedAt = DateTime.Now;
        }
    }

    #region Tables

    internal DbSet<FieldTable> Fields { get; set; }
    internal DbSet<ScenarioFieldTable> ScenarioFields { get; set; }
    internal DbSet<ScenarioTable> Scenarios { get; set; }
    internal DbSet<TagTable> Tags { get; set; }

    #endregion
}
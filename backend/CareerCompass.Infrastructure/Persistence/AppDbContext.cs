using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Fields;
using CareerCompass.Infrastructure.Scenarios;
using CareerCompass.Infrastructure.Tags;
using CareerCompass.Infrastructure.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CareerCompass.Infrastructure.Persistence;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Define the ScenarioId foreign key relationship
        modelBuilder.Entity<ScenarioFieldTable>()
            .HasOne(s => s.Scenario)
            .WithMany()
            .HasForeignKey(s => s.ScenarioId)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete

        // Define the FieldId foreign key relationship
        modelBuilder.Entity<ScenarioFieldTable>()
            .HasOne(f => f.Field)
            .WithMany()
            .HasForeignKey(f => f.FieldId)
            .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete
        
        modelBuilder.Entity<ScenarioTable>()
            .HasMany(s => s.Tags)
            .WithMany(t => t.Scenarios)
            .UsingEntity<Dictionary<string, object>>(
                "ScenarioTags",
                j => j
                    .HasOne<TagTable>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.Restrict), // Prevent cascade delete on Tags
                j => j
                    .HasOne<ScenarioTable>()
                    .WithMany()
                    .HasForeignKey("ScenarioId")
                    .OnDelete(DeleteBehavior.Restrict) // Prevent cascade delete on Scenarios
            );

    }

    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        if (entity is not IdentityUser) return base.Add(entity);

        var agent = new AgentTable
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        Add(agent);
        return base.Add(entity);
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
    internal DbSet<AgentTable> Agents { get; set; }

    #endregion
}
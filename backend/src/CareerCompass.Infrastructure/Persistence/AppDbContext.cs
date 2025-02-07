using CareerCompass.Infrastructure.Common;
using CareerCompass.Infrastructure.Persistence.Fields;
using CareerCompass.Infrastructure.Persistence.Scenarios;
using CareerCompass.Infrastructure.Persistence.Tags;
using CareerCompass.Infrastructure.Persistence.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CareerCompass.Infrastructure.Persistence;

internal class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AgentTable>()
            .HasOne<IdentityUser>(e => e.User)
            .WithOne();
        modelBuilder.Entity<AgentTable>()
            .HasOne(u => u.User) // User has one UserProfile
            .WithOne() // UserProfile has one User
            .HasForeignKey<AgentTable>(up => up.UserId); // FK in UserProfile


        modelBuilder.Entity<ScenarioFieldTable>(entity =>
        {
            entity.HasKey(e => e.Id); // Primary Key

            // Configure Scenario relationship
            entity.HasOne(e => e.Scenario)
                .WithMany(s => s.ScenarioFields) // This links back to ScenarioTable's navigation property
                .HasForeignKey(e => e.ScenarioId)
                ;

            // Configure Field relationship
            entity.HasOne(e => e.Field)
                .WithMany(f => f.ScenarioFields) // This links back to FieldTable's navigation property
                .HasForeignKey(e => e.FieldId);
        });
        // Configuration in ScenarioTable
        modelBuilder.Entity<ScenarioTable>()
            .HasMany(s => s.ScenarioFields)
            .WithOne(sf => sf.Scenario)
            .HasForeignKey(sf => sf.ScenarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuration in FieldTable
        modelBuilder.Entity<FieldTable>()
            .HasMany(f => f.ScenarioFields)
            .WithOne(sf => sf.Field)
            .HasForeignKey(sf => sf.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

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
        if (entity is IdentityUser user)
        {
            Add(new AgentTable
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = user.Id,
                FirstName = "",
                LastName = "",
            });
        }


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
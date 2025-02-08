using CareerCompass.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerCompass.Infrastructure.Persistence.Configurations;

internal class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("AspNetUsers"); // Identity table name

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, s => UserId.Create(s));


        builder.Property(e => e.FirstName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .HasMaxLength(100);
    }
}

internal class ApplicationIdentityUserConfigurations : IEntityTypeConfiguration<ApplicationIdentityUser>
{
    public void Configure(EntityTypeBuilder<ApplicationIdentityUser> builder)
    {
        builder.ToTable("AspNetUsers");
        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<ApplicationIdentityUser>(e => e.Id); // Shared primary key

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, s => UserId.Create(s));
    }
}
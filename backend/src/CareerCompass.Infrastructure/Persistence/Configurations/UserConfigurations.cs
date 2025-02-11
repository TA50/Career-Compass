using CareerCompass.Core.Common;
using CareerCompass.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerCompass.Infrastructure.Persistence.Configurations;

internal class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));


        builder.Property(e => e.FirstName)
            .HasMaxLength(Limits.MaxNameLength);

        builder.Property(e => e.LastName)
            .HasMaxLength(Limits.MaxNameLength);

        builder.Property(e => e.Email)
            .HasMaxLength(Limits.MaxEmailLength);

        builder.Property(e => e.Password)
            .HasMaxLength(Limits.MaxPasswordLength);

        builder.Property(e => e.EmailConfirmationCode)
            .HasMaxLength(Limits.EmailConfirmationCodeLength);
    }
}
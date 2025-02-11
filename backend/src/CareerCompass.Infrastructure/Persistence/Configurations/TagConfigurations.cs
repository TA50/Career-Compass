using CareerCompass.Core.Common;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerCompass.Infrastructure.Persistence.Configurations;

internal class TagConfigurations : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => TagId.Create(value));

        builder.Property(t => t.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));


        builder.Property(t => t.Name)
            .HasMaxLength(Limits.MaxNameLength)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Name })
            .IsUnique();
    }
}
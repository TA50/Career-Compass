using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerCompass.Infrastructure.Persistence.Configurations;

internal class FieldConfigurations : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => FieldId.Create(value));

        builder.Property(t => t.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));


        builder.Property(t => t.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.Group)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Name, x.Group })
            .IsUnique();
    }
}
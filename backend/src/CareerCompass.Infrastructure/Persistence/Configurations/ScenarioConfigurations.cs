using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerCompass.Infrastructure.Persistence.Configurations;

internal class ScenarioConfigurations : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        ConfigureScenariosTable(builder);
        ConfigureScenarioFieldsTable(builder);
        ConfigureTagIdsTable(builder);
    }

    private void ConfigureScenariosTable(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("Scenarios");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, s => ScenarioId.Create(s));

        builder.Property(s => s.Title)
            .HasMaxLength(100);

        builder.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, s => UserId.Create(s));
    }

    private void ConfigureScenarioFieldsTable(EntityTypeBuilder<Scenario> builder)
    {
        builder.OwnsMany(e => e.ScenarioFields, sfb =>
        {
            sfb.ToTable("ScenarioFields");
            sfb.WithOwner()
                .HasForeignKey(nameof(ScenarioId));

            sfb.HasKey(nameof(ScenarioField.Id), nameof(ScenarioId), nameof(FieldId));

            sfb.Property(e => e.Id)
                .HasColumnName(nameof(ScenarioFieldId))
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, s => ScenarioFieldId.Create(s));

            sfb.Property(e => e.FieldId)
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, s => FieldId.Create(s));
        });

        // builder.Navigation(e => e.ScenarioFields).Metadata.SetField("_scenarioFields");
        var nav = builder.Metadata.FindNavigation(nameof(Scenario.ScenarioFields));
        if (nav is null)
        {
            throw new DatabaseOperationException(
                $"{nameof(ScenarioConfigurations)}: Navigation property ( {nameof(Scenario.ScenarioFields)} ) was not found");
        }

        nav.SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureTagIdsTable(EntityTypeBuilder<Scenario> builder)
    {
        builder.OwnsMany(s => s.TagIds, tib =>
        {
            tib.ToTable("ScenarioTagIds");

            tib.WithOwner()
                .HasForeignKey("ScenarioId");


            tib.HasKey("Id");

            tib.Property(t => t.Value)
                .HasColumnName("ScenarioTagId")
                .ValueGeneratedNever();
        });

        var nav = builder.Metadata.FindNavigation(nameof(Scenario.TagIds));
        if (nav is null)
        {
            throw new DatabaseOperationException(
                $"{nameof(ScenarioConfigurations)}: Navigation property ( {nameof(Scenario.TagIds)} ) was not found");
        }

        nav.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(t => new { t.WorkspaceId, t.Name }).IsUnique();

        // Same reasoning as Page.Aliases (see PageConfiguration): a getter-only
        // collection property is enough for EF's convention to try mapping
        // TagPropertyTemplate as a navigable entity unless explicitly ignored.
        builder.Ignore(t => t.PropertyTemplates);

        var templatesProperty = builder.Property<List<TagPropertyTemplate>>("_propertyTemplates")
            .HasConversion(
                templates => JsonSerializer.Serialize(templates.Select(t => new TagPropertyTemplateRecord(t.Key, t.Type, t.DefaultValue, t.AllowMultiple)), (JsonSerializerOptions?)null),
                json => (JsonSerializer.Deserialize<List<TagPropertyTemplateRecord>>(json, (JsonSerializerOptions?)null) ?? new List<TagPropertyTemplateRecord>())
                    .Select(r => new TagPropertyTemplate(r.Key, r.Type, r.DefaultValue, r.AllowMultiple))
                    .ToList())
            .HasColumnName("PropertyTemplates")
            .HasColumnType("jsonb");

        templatesProperty.Metadata.SetValueComparer(new ValueComparer<List<TagPropertyTemplate>>(
            (a, b) => a!.SequenceEqual(b!),
            a => a.Aggregate(0, (hash, t) => HashCode.Combine(hash, t)),
            a => a.ToList()));
    }

    private record TagPropertyTemplateRecord(string Key, SecundMemory.Domain.Properties.PropertyType Type, string? DefaultValue, bool AllowMultiple);
}

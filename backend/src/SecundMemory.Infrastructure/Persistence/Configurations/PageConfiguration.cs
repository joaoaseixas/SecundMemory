using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    // Computed rather than written as a literal in source, to avoid any
    // ambiguity about which control character ends up in the file: ASCII
    // code point 31 (Unit Separator), which never appears in ordinary text.
    private static readonly string AliasDelimiter = Convert.ToChar(31).ToString();

    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.Property(p => p.Title)
            .HasConversion(title => title.Value, value => PageTitle.Create(value))
            .HasMaxLength(PageTitle.MaxLength)
            .IsRequired();

        builder.HasIndex(p => p.WorkspaceId);
        builder.HasIndex("WorkspaceId", "Title");

        // Aliases is a computed projection over _aliasValues, not itself
        // persisted — without this, EF Core's convention still discovers it
        // as a PageAlias collection navigation purely from the property's
        // return type, regardless of the backing field.
        builder.Ignore(p => p.Aliases);

        var aliasesProperty = builder.Property<List<string>>("_aliasValues")
            .HasConversion(
                values => string.Join(AliasDelimiter, values),
                raw => raw.Length == 0 ? new List<string>() : raw.Split(AliasDelimiter, StringSplitOptions.None).ToList())
            .HasColumnName("Aliases");

        aliasesProperty.Metadata.SetValueComparer(new ValueComparer<List<string>>(
            (a, b) => a!.SequenceEqual(b!),
            a => a.Aggregate(0, HashCode.Combine),
            a => a.ToList()));

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}

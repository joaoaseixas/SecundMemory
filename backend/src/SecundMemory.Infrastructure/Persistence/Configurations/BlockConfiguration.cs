using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.Property(b => b.Content)
            .HasConversion(content => content.Value, value => BlockContent.Create(value))
            .HasMaxLength(BlockContent.MaxLength)
            .IsRequired();

        builder.Property(b => b.Type).HasConversion<string>().HasMaxLength(32);

        builder.Property<List<Guid>>("_tagIds")
            .HasConversion(ValueConverters.GuidListAsJson)
            .HasColumnName("TagIds")
            .HasColumnType("jsonb")
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Guid>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, id) => HashCode.Combine(hash, id)),
                a => a.ToList()));

        builder.HasIndex(b => b.PageId);
        builder.HasIndex(b => b.ParentBlockId);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}

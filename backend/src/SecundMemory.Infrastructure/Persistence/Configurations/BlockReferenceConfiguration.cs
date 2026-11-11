using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Links;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class BlockReferenceConfiguration : IEntityTypeConfiguration<BlockReference>
{
    public void Configure(EntityTypeBuilder<BlockReference> builder)
    {
        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(32);

        builder.HasIndex(r => r.SourceBlockId);
        builder.HasIndex(r => r.TargetPageId);
        builder.HasIndex(r => r.TargetBlockId);
    }
}

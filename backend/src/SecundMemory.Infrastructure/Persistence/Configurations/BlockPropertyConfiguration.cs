using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Properties;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class BlockPropertyConfiguration : IEntityTypeConfiguration<BlockProperty>
{
    public void Configure(EntityTypeBuilder<BlockProperty> builder)
    {
        builder.Property(p => p.Key).HasMaxLength(128).IsRequired();
        builder.Property(p => p.Type).HasConversion<string>().HasMaxLength(32);

        builder.HasIndex(p => p.BlockId);
        builder.HasIndex(p => new { p.BlockId, p.Key }).IsUnique();
    }
}

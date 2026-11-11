using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Attachments;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.Property(a => a.FileName).HasMaxLength(256).IsRequired();
        builder.Property(a => a.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(a => a.StorageKey).HasMaxLength(512).IsRequired();

        builder.HasIndex(a => a.WorkspaceId);
        builder.HasIndex(a => a.BlockId);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

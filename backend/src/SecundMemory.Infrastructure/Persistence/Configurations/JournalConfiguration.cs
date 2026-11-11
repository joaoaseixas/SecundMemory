using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Journals;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class JournalConfiguration : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.HasIndex(j => new { j.WorkspaceId, j.Date }).IsUnique();
    }
}

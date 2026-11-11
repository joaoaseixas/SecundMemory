using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.Property(w => w.Name).HasMaxLength(128).IsRequired();

        builder.HasMany(w => w.Members)
            .WithOne()
            .HasForeignKey(m => m.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(w => w.Members).HasField("_members").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.Property(m => m.Role).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(m => new { m.WorkspaceId, m.UserId }).IsUnique();
    }
}

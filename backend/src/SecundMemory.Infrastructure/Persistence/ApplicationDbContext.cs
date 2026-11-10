using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Interfaces;
using SecundMemory.Domain.Entities;

namespace SecundMemory.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ProfileFact> ProfileFacts => Set<ProfileFact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.Property(n => n.Title).IsRequired().HasMaxLength(256);
            entity.HasMany(n => n.Tags).WithMany(t => t.Notes).UsingEntity(j => j.ToTable("NoteTags"));
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(64);
            entity.HasIndex(t => t.Name).IsUnique();
        });

        modelBuilder.Entity<ProfileFact>(entity =>
        {
            entity.Property(f => f.Category).IsRequired().HasMaxLength(64);
            entity.Property(f => f.Key).IsRequired().HasMaxLength(128);
            entity.HasIndex(f => new { f.Category, f.Key }).IsUnique();
        });
    }
}

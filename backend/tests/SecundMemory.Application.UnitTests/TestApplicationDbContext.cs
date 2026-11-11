using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Domain.Attachments;
using SecundMemory.Domain.Journals;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Properties;
using SecundMemory.Domain.Tags;
using SecundMemory.Domain.Users;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.UnitTests;

/// <summary>
/// In-memory stand-in for IApplicationDbContext used only by Application-layer
/// tests, so those tests never depend on the real Infrastructure/Npgsql
/// implementation — proving the use cases are testable in isolation.
/// </summary>
public class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<BlockProperty> BlockProperties => Set<BlockProperty>();
    public DbSet<Journal> Journals => Set<Journal>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<BlockReference> BlockReferences => Set<BlockReference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workspace>(e =>
        {
            e.HasMany(w => w.Members).WithOne().HasForeignKey(m => m.WorkspaceId);
            e.Navigation(w => w.Members).HasField("_members").UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Email).HasConversion(email => email.Value, value => Email.Create(value));
        });

        modelBuilder.Entity<Tag>(e =>
        {
            e.Ignore(t => t.PropertyTemplates);
            e.Property<List<TagPropertyTemplate>>("_propertyTemplates").HasConversion(
                templates => JsonSerializer.Serialize(
                    templates.Select(t => new TemplateRecord(t.Key, t.Type, t.DefaultValue, t.AllowMultiple)), (JsonSerializerOptions?)null),
                json => (JsonSerializer.Deserialize<List<TemplateRecord>>(json, (JsonSerializerOptions?)null) ?? new List<TemplateRecord>())
                    .Select(r => new TagPropertyTemplate(r.Key, r.Type, r.DefaultValue, r.AllowMultiple))
                    .ToList());
        });
    }

    private record TemplateRecord(string Key, PropertyType Type, string? DefaultValue, bool AllowMultiple);

    public static TestApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}

using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Domain.Attachments;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Journals;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;
using SecundMemory.Domain.Properties;
using SecundMemory.Domain.Tags;
using SecundMemory.Domain.Users;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    // Exposed directly to Infrastructure.Persistence.Repositories only —
    // Application reaches Page/Block through IPageRepository/IBlockRepository,
    // never through these DbSets (see IApplicationDbContext, which omits them).
    internal DbSet<Page> Pages => Set<Page>();
    internal DbSet<Block> Blocks => Set<Block>();

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<BlockProperty> BlockProperties => Set<BlockProperty>();
    public DbSet<Journal> Journals => Set<Journal>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<BlockReference> BlockReferences => Set<BlockReference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

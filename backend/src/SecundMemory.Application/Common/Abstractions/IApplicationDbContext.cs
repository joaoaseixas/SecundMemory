using Microsoft.EntityFrameworkCore;
using SecundMemory.Domain.Attachments;
using SecundMemory.Domain.Journals;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Properties;
using SecundMemory.Domain.Tags;
using SecundMemory.Domain.Users;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.Common.Abstractions;

/// <summary>
/// Direct DbSet access for the aggregates that don't warrant a dedicated
/// repository (Page and Block do — see IPageRepository/IBlockRepository).
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Workspace> Workspaces { get; }
    DbSet<User> Users { get; }
    DbSet<Tag> Tags { get; }
    DbSet<BlockProperty> BlockProperties { get; }
    DbSet<Journal> Journals { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<BlockReference> BlockReferences { get; }
}

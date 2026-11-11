namespace SecundMemory.Domain.Common;

/// <summary>
/// Created/modified stamps are populated by
/// Infrastructure.Persistence.Interceptors.AuditableEntityInterceptor on save —
/// entities never set these themselves.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; internal set; }
    public Guid? CreatedBy { get; internal set; }
    public DateTime? LastModifiedAtUtc { get; internal set; }
    public Guid? LastModifiedBy { get; internal set; }
}

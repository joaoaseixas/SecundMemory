namespace SecundMemory.Domain.Entities;

/// <summary>
/// A durable fact about the user (preferences, bio details, context an AI
/// assistant should already know) so it never has to be re-typed into a new
/// AI conversation or service.
/// </summary>
public class ProfileFact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Category { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}

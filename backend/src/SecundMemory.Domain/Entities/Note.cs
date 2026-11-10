namespace SecundMemory.Domain.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Tag> Tags { get; set; } = [];
}

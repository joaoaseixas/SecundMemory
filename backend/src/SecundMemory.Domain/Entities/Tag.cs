namespace SecundMemory.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    public ICollection<Note> Notes { get; set; } = [];
}

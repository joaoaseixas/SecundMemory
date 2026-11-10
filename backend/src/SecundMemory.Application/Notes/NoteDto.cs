namespace SecundMemory.Application.Notes;

public record NoteDto(
    Guid Id,
    string Title,
    string Content,
    IReadOnlyList<string> Tags,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);

public record CreateNoteRequest(string Title, string Content, IReadOnlyList<string> Tags);

public record UpdateNoteRequest(string Title, string Content, IReadOnlyList<string> Tags);

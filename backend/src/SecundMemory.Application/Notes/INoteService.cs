namespace SecundMemory.Application.Notes;

public interface INoteService
{
    Task<IReadOnlyList<NoteDto>> SearchAsync(string? query, CancellationToken cancellationToken = default);
    Task<NoteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NoteDto> CreateAsync(CreateNoteRequest request, CancellationToken cancellationToken = default);
    Task<NoteDto?> UpdateAsync(Guid id, UpdateNoteRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

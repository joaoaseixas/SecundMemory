using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Interfaces;
using SecundMemory.Domain.Entities;

namespace SecundMemory.Application.Notes;

public class NoteService(IApplicationDbContext db) : INoteService
{
    public async Task<IReadOnlyList<NoteDto>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var notes = db.Notes.Include(n => n.Tags).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            notes = notes.Where(n => n.Title.Contains(query) || n.Content.Contains(query));
        }

        var results = await notes.OrderByDescending(n => n.UpdatedUtc).ToListAsync(cancellationToken);
        return results.Select(ToDto).ToList();
    }

    public async Task<NoteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await db.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        return note is null ? null : ToDto(note);
    }

    public async Task<NoteDto> CreateAsync(CreateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var note = new Note { Title = request.Title, Content = request.Content };
        note.Tags = await ResolveTagsAsync(request.Tags, cancellationToken);

        db.Notes.Add(note);
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(note);
    }

    public async Task<NoteDto?> UpdateAsync(Guid id, UpdateNoteRequest request, CancellationToken cancellationToken = default)
    {
        var note = await db.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (note is null) return null;

        note.Title = request.Title;
        note.Content = request.Content;
        note.Tags = await ResolveTagsAsync(request.Tags, cancellationToken);
        note.UpdatedUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return ToDto(note);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var note = await db.Notes.FindAsync([id], cancellationToken);
        if (note is null) return false;

        db.Notes.Remove(note);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<List<Tag>> ResolveTagsAsync(IReadOnlyList<string> tagNames, CancellationToken cancellationToken)
    {
        var distinctNames = tagNames.Select(t => t.Trim()).Where(t => t.Length > 0).Distinct().ToList();
        if (distinctNames.Count == 0) return [];

        var existing = await db.Tags.Where(t => distinctNames.Contains(t.Name)).ToListAsync(cancellationToken);
        var missing = distinctNames.Except(existing.Select(t => t.Name))
            .Select(name => new Tag { Name = name })
            .ToList();

        db.Tags.AddRange(missing);
        return existing.Concat(missing).ToList();
    }

    private static NoteDto ToDto(Note note) => new(
        note.Id,
        note.Title,
        note.Content,
        note.Tags.Select(t => t.Name).ToList(),
        note.CreatedUtc,
        note.UpdatedUtc);
}

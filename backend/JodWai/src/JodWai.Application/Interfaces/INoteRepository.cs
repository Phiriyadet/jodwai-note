using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Interfaces;

public interface INoteRepository
{
    Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default);
    Task<Guid?> GetIdByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Note>> GetNotesReferencingAsync(NoteId noteId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Note>> SearchAsync(
    string keyword,
    CancellationToken cancellationToken);
    void Update(Note note);
    void Delete(Note note);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

using JodWai.Application.Common.Pagination;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Interfaces;

public interface INoteRepository
{
    Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default);
    Task<Guid?> GetIdByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Note>> GetNotesReferencingAsync(NoteId noteId, CancellationToken cancellationToken);
    Task<PagedResult<Note>> GetPagedAsync(
        GetNotesOptions options,
        CancellationToken cancellationToken = default);
    void Update(Note note);
    void Delete(Note note);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

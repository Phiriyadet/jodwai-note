using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Interfaces;

public interface INoteRepository
{
    Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default);
    Task<Guid?> GetIdByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<IEnumerable<Note>> GetAllAsync(CancellationToken cancellationToken = default);
    void Update(Note note);
    void Delete(Note note);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

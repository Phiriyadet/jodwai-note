using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Interfaces;

public interface INoteLinkResolver
{
    Task<IReadOnlyList<NoteId>> ResolveAsync(
        NoteContent content,
        CancellationToken cancellationToken);
}

using JodWai.Application.Interfaces;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Services;

public sealed class NoteLinkResolver : INoteLinkResolver
{
    private readonly INoteRepository _noteRepository;
    private readonly INoteLinkParser _parser;

    public NoteLinkResolver(
        INoteRepository noteRepository,
        INoteLinkParser parser)
    {
        _noteRepository = noteRepository;
        _parser = parser;
    }

    public async Task<IReadOnlyList<NoteId>> ResolveAsync(
        NoteContent content,
        CancellationToken cancellationToken)
    {
        var linkIds = new List<NoteId>();

        var linkTitles = _parser.Parse(content)
            .Select(x => x.TargetTitle);

        foreach (var title in linkTitles)
        {
            var existingId = await _noteRepository
                .GetIdByTitleAsync(title, cancellationToken);

            if (existingId.HasValue)
            {
                linkIds.Add(NoteId.From(existingId.Value));
                continue;
            }

            var blankNote = Note.Create(
                NoteTitle.From(title),
                NoteContent.Empty);

            var created = await _noteRepository.CreateAsync(
                blankNote,
                cancellationToken);

            linkIds.Add(created.Id);
        }

        return linkIds;
    }
}

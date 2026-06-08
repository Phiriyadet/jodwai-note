using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Commands;

public record CreateNoteCommand(CreateNoteRequest Request) : IRequest<NoteDto>;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly INoteLinkParser _parser;

    public CreateNoteCommandHandler(INoteRepository noteRepository, INoteLinkParser parser)
    {
        _noteRepository = noteRepository;
        _parser = parser;
    }

    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        // 1. Prepare the new note
        var title = NoteTitle.From(request.Request.Title);
        var content = NoteContent.From(request.Request.Content);

        var existingTitle = await _noteRepository.GetIdByTitleAsync(title.Value, cancellationToken);

        if (existingTitle != null)
        {
            throw new InvalidOperationException("Title already exists.");
        }


        var note = Note.Create(
            title: title,
            content: content);

        // 2. Process links (create missing notes or find existing ones)
        var linkIds = await _processLinksAsync(content, cancellationToken);

        // 3. Attach link IDs to the note
        note.SyncLinks(linkIds);

        // 4. Persist the note
        var createdNote = await _noteRepository.CreateAsync(note, cancellationToken);
        await _noteRepository.SaveChangesAsync(cancellationToken);

        return createdNote.ToDto();
    }

    private async Task<List<NoteId>> _processLinksAsync(NoteContent content, CancellationToken cancellationToken)
    {
        // Parse links from content
        var parsedLinkTitles = _parser.Parse(content)
            .Select(link => link.TargetTitle);

        var linkIds = new List<NoteId>();

        foreach (var title in parsedLinkTitles)
        {
            // Try to find existing note by title
            var existingId = await _noteRepository.GetIdByTitleAsync(title, cancellationToken);

            if (existingId.HasValue)
            {
                // Link exists: Add ID
                linkIds.Add(NoteId.From(existingId.Value));
            }
            else
            {
                // Link missing: Create a new blank note with the same title
                var newBlankNote = Note.Create(
                    title: NoteTitle.From(title),
                    content: NoteContent.From("")); // Reuse or create a blank content

                var createdId = await _noteRepository.CreateAsync(newBlankNote, cancellationToken);
                linkIds.Add(createdId.Id);
            }
        }

        return linkIds;
    }
}

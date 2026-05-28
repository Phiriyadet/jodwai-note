using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Commands;

public record UpdateNoteCommand(UpdateNoteRequest Request) : IRequest<NoteDto>;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    private readonly INoteLinkParser _parser;
    public UpdateNoteCommandHandler(INoteRepository noteRepository, INoteLinkParser parser)
    {
        _noteRepository = noteRepository;
        _parser = parser;
    }
    public async Task<NoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var noteId = NoteId.From(request.Request.Id);
        var note = await _noteRepository.GetByIdAsync(noteId);

        if (note is null)
        {
            throw new KeyNotFoundException("Note not found.");
        }

        var title = NoteTitle.From(request.Request.Title);
        var content = NoteContent.From(request.Request.Content);

        var existingTitle = await _noteRepository.GetIdByTitleAsync(title.Value, cancellationToken);

        if (existingTitle != null)
        {
            throw new InvalidOperationException("Title already exists.");
        }

        note.Update(
            title: NoteTitle.From(request.Request.Title),
            content: content);

        var linkIds = await _processLinksAsync(content, cancellationToken);

        note.SyncLinks(linkIds);

        _noteRepository.Update(note);

        await _noteRepository.SaveChangesAsync();

        return note.ToDto();
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


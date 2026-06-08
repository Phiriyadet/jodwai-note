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
        var note = await _noteRepository.GetByIdAsync(noteId, cancellationToken);

        if (note is null)
        {
            throw new KeyNotFoundException($"Note with id {noteId} not found.");
        }

        var oldTitle = note.Title;
        var newTitle = request.Request.Title is { Length: > 0 }
            ? NoteTitle.From(request.Request.Title)
            : null;
        var newContent = request.Request.Content is { Length: > 0 }
            ? NoteContent.From(request.Request.Content)
            : null;

        var titleChanged = newTitle is not null && oldTitle.Value != newTitle.Value;

        //Before update — guard against duplicate title conflict
        if (titleChanged)
        {
            var existingTitle = await _noteRepository
                .GetIdByTitleAsync(newTitle!.Value, cancellationToken);

            if (existingTitle.HasValue)
            {
                throw new InvalidOperationException(
                    $"Title '{newTitle.Value}' already exists in the system.");
            }
        }

        note.Update(
            title: newTitle,
            content: newContent);

        var linkIds = await _processLinksAsync(newContent, cancellationToken);
        note.SyncLinks(linkIds);

        _noteRepository.Update(note);

        //After update — propagate the rename to all referencing notes
        if (titleChanged)
        {
            var notesRaf = await _noteRepository
                .GetNotesReferencingAsync(note.Id, cancellationToken);

            foreach (var noteRaf in notesRaf)
            {
                var contentWithUpdatedLinks = ReplaceLinkTitlesInContent(
                    noteRaf.Content,
                    oldTitle,
                    newTitle);

                noteRaf.Update(
                    title: null,
                    content: contentWithUpdatedLinks);
                _noteRepository.Update(noteRaf);
            }
        }

        await _noteRepository.SaveChangesAsync(cancellationToken);

        return note.ToDto();
    }

    private async Task<List<NoteId>> _processLinksAsync(
        NoteContent? content,
        CancellationToken cancellationToken)
    {
        if (content is null)
        {
            return [];
        }

        var parsedLinkTitles = _parser.Parse(content)
            .Select(link => link.TargetTitle);

        var linkIds = new List<NoteId>();

        foreach (var title in parsedLinkTitles)
        {
            var existingId = await _noteRepository
                .GetIdByTitleAsync(title, cancellationToken);

            if (existingId.HasValue)
            {
                linkIds.Add(NoteId.From(existingId.Value));
            }
            else
            {
                var newBlankNote = Note.Create(
                    title: NoteTitle.From(title),
                    content: NoteContent.From(string.Empty));

                var createdNote = await _noteRepository
                    .CreateAsync(newBlankNote, cancellationToken);

                linkIds.Add(createdNote.Id);
            }
        }

        return linkIds;
    }

    private NoteContent ReplaceLinkTitlesInContent(
        NoteContent content,
        NoteTitle oldTitle,
        NoteTitle? newTitle)
    {
        if (newTitle is null || oldTitle.Value == newTitle.Value)
        {
            return content;
        }

        var updatedContent = content.Value
            .Replace($"[[{oldTitle.Value}]]", $"[[{newTitle.Value}]]");

        return NoteContent.From(updatedContent);
    }
}


using JodWai.Application.Common.Results;
using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Commands.UpdateNote;

public record UpdateNoteCommand(UpdateNoteRequest Request) : IRequest<Result<NoteDto>>;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Result<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly INoteLinkResolver _noteLinkResolver;

    public UpdateNoteCommandHandler(INoteRepository noteRepository, INoteLinkResolver resolver)
    {
        _noteRepository = noteRepository;
        _noteLinkResolver = resolver;
    }

    public async Task<Result<NoteDto>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var noteId = NoteId.From(request.Request.Id);
        var note = await _noteRepository.GetByIdAsync(noteId, cancellationToken);

        if (note is null)
        {
            return Result<NoteDto>.Failure(NoteErrors.NotFound(noteId.Value));
        }

        var oldTitle = note.Title;
        var newTitle = request.Request.Title is { Length: > 0 }
            ? NoteTitle.From(request.Request.Title)
            : null;
        var newContent = request.Request.Content.GetRawText() is { Length: > 0 }
            ? NoteContent.From(request.Request.Content.GetRawText())
            : null;

        if (newTitle is null && newContent is null)
        {
            // Nothing to update
            return Result<NoteDto>.Success(note.ToDto());
        }

        var titleChanged = newTitle is not null && oldTitle.Value != newTitle.Value;

        //Before update — guard against duplicate title conflict
        if (titleChanged)
        {
            var existingTitle = await _noteRepository
                .GetIdByTitleAsync(newTitle!.Value, cancellationToken);

            if (existingTitle.HasValue)
            {
                return Result<NoteDto>.Failure(NoteErrors.DuplicateTitle(newTitle.Value));
            }
        }

        note.Update(
            title: newTitle,
            content: newContent);

        var linkIds = await _noteLinkResolver
            .ResolveAsync(note.Content, cancellationToken);
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

        return Result<NoteDto>.Success(note.ToDto());
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


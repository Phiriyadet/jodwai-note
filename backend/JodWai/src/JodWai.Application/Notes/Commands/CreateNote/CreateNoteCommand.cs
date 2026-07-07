using JodWai.Application.Common.Results;
using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Extensions;
using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Commands.CreateNote;

public record CreateNoteCommand(CreateNoteRequest Request) : IRequest<Result<NoteDto>>;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    private readonly INoteLinkResolver _noteLinkResolver;

    public CreateNoteCommandHandler(INoteRepository noteRepository, INoteLinkResolver resolver)

    {
        _noteRepository = noteRepository;
        _noteLinkResolver = resolver;
    }

    public async Task<Result<NoteDto>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        // 1. Prepare the new note
        var title = NoteTitle.From(request.Request.Title);
        var content = NoteContent.From(request.Request.Content.ToNoteContentJson());

        var existingTitle = await _noteRepository.GetIdByTitleAsync(title.Value, cancellationToken);

        if (existingTitle != null)
        {
            return Result<NoteDto>.Failure(NoteErrors.DuplicateTitle(title.Value));
        }


        var note = Note.Create(
            title: title,
            content: content);

        // 2. Process links (create missing notes or find existing ones)
        var linkIds = await _noteLinkResolver
            .ResolveAsync(note.Content, cancellationToken);

        // 3. Attach link IDs to the note
        note.SyncLinks(linkIds);

        // 4. Persist the note
        var createdNote = await _noteRepository.CreateAsync(note, cancellationToken);
        await _noteRepository.SaveChangesAsync(cancellationToken);

        return Result<NoteDto>.Success(createdNote.ToDto());
    }
}

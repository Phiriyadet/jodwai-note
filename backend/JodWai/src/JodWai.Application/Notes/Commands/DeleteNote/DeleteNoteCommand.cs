using JodWai.Application.Common.Results;
using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Interfaces;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Commands.DeleteNote;

public record DeleteNoteCommand(Guid Id) : IRequest<Result>;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand,Result>
{
    private readonly INoteRepository _noteRepository;
    public DeleteNoteCommandHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var noteId = NoteId.From(request.Id);

        var note = await _noteRepository.GetByIdAsync(noteId);

        if (note is null)
        {
            return Result.Failure(NoteErrors.NotFound(request.Id));
        }

        _noteRepository.Delete(note);

        await _noteRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


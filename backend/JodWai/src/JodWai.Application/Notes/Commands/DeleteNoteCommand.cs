using JodWai.Application.Interfaces;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Commands;

public record DeleteNoteCommand(Guid Id) : IRequest;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
{
    private readonly INoteRepository _noteRepository;
    public DeleteNoteCommandHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var noteId = NoteId.From(request.Id);

        var note = await _noteRepository.GetByIdAsync(noteId);

        if (note is null)
        {
            throw new KeyNotFoundException("Note not found.");
        }

        _noteRepository.Delete(note);

        await _noteRepository.SaveChangesAsync();
    }
}


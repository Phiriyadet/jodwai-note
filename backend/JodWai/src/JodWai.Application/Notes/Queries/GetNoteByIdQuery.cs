using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Queries;

public record GetNoteByIdQuery(Guid Id) : IRequest<NoteDto>;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto>
{
    private readonly INoteRepository _noteRepository;
    public GetNoteByIdQueryHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task<NoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var noteId = NoteId.From(request.Id);

        var note = await _noteRepository.GetByIdAsync(noteId);

        if (note is null)
        {
            throw new KeyNotFoundException("Note not found.");
        }

        return note.ToDto();
    }
}

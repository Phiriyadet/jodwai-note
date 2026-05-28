using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;

using MediatR;

namespace JodWai.Application.Notes.Queries;

public record GetAllNotesQuery() : IRequest<IEnumerable<NoteDto>>;

public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, IEnumerable<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    public GetAllNotesQueryHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task<IEnumerable<NoteDto>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.GetAllAsync();

        return notes.ToDto();
    }
}

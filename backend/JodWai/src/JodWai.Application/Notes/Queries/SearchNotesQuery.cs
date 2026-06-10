using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;

using MediatR;

namespace JodWai.Application.Notes.Queries;

public sealed record SearchNotesQuery(string Keyword)
    : IRequest<IReadOnlyList<NoteDto>>;

public class SearchNotesQueryHandler : IRequestHandler<SearchNotesQuery, IReadOnlyList<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    public SearchNotesQueryHandler(INoteRepository noteRepository) { _noteRepository = noteRepository; }

    public async Task<IReadOnlyList<NoteDto>> Handle(SearchNotesQuery request, CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.SearchAsync(request.Keyword, cancellationToken);

        return notes.ToDto();
    }
}

using JodWai.Application.Common.Pagination;
using JodWai.Application.Common.Results;
using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;

using MediatR;

namespace JodWai.Application.Notes.Queries.GetNotes;

public record GetNotesQuery(GetNotesRequest Request) : IRequest<Result<PagedResult<NoteDto>>>;

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, Result<PagedResult<NoteDto>>>
{
    private readonly INoteRepository _noteRepository;
    public GetNotesQueryHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task<Result<PagedResult<NoteDto>>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
    {
        var options = new GetNotesOptions(
            request.Request.Page,
            request.Request.PageSize,
            request.Request.Search,
            request.Request.CreatedAfter,
            request.Request.CreatedBefore,
            request.Request.SortBy,
            request.Request.SortOrder
        );

        var pagedNotes = await _noteRepository.GetPagedAsync(options, cancellationToken);

        var result = new PagedResult<NoteDto>(
            Items: pagedNotes.Items.ToDto(),
            Page: pagedNotes.Page,
            PageSize: pagedNotes.PageSize,
            TotalItems: pagedNotes.TotalItems
        );

        return Result<PagedResult<NoteDto>>.Success(result);
    }
}

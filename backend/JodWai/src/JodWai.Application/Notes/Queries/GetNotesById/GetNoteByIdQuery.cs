using JodWai.Application.Common.Results;
using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Interfaces;
using JodWai.Application.Mappers;
using JodWai.Application.Notes.Dtos;
using JodWai.Domain.ValueObjects;

using MediatR;

namespace JodWai.Application.Notes.Queries.GetNoteById;

public record GetNoteByIdQuery(Guid Id) : IRequest<Result<NoteDto>>;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Result<NoteDto>>
{
    private readonly INoteRepository _noteRepository;
    public GetNoteByIdQueryHandler(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task<Result<NoteDto>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var noteId = NoteId.From(request.Id);

        var note = await _noteRepository.GetByIdAsync(noteId, cancellationToken);

        if (note is null)
        {
            return Result<NoteDto>.Failure(NoteErrors.NotFound(noteId.Value));
        }

        return Result<NoteDto>.Success(note.ToDto());
    }
}

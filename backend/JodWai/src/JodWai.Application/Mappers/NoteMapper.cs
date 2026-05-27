using JodWai.Application.Notes.Dtos;
using JodWai.Domain.Entities;

namespace JodWai.Application.Mappers;

public static class NoteMapper
{
    public static NoteDto ToDto(this Note note)
    {
        return new NoteDto(
            note.Id.Value,
            note.Title.ToString(),
            note.Content.ToString(),
            note.CreatedAt,
            note.UpdatedAt
        );
    }

    public static IEnumerable<NoteDto> ToDto(this IEnumerable<Note> notes)
    {
        return notes.Select(n => n.ToDto());
    }
}

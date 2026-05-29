using JodWai.Application.Notes.Dtos;
using JodWai.Domain.Entities;

namespace JodWai.Application.Mappers;

public static class NoteMapper
{
    public static NoteDto ToDto(this Note note)
    {
        return new NoteDto(
            Id: note.Id.Value,
            Title: note.Title.ToString(),
            Content: note.Content.ToString(),
            Links: note.Links,
            Tags: note.Tags,
            CreatedAt: note.CreatedAt,
            UpdatedAt: note.UpdatedAt
        );
    }

    public static IEnumerable<NoteDto> ToDto(this IEnumerable<Note> notes)
    {
        return notes.Select(n => n.ToDto());
    }
}

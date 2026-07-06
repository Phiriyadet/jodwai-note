using System.Text.Json;

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
            Content: JsonSerializer
                .Deserialize<JsonElement>(
                    note.Content.ToString()),
            Links: note.Links
                .Select(link =>
                new NoteLinkDto(
                    link.TargetId.Value))
                    .ToList(),
            Tags: note.Tags
                .Select(tag => tag
                .ToString())
                .ToList(),
            CreatedAt: note.CreatedAt,
            UpdatedAt: note.UpdatedAt
        );
    }

    public static IReadOnlyList<NoteDto> ToDto(
    this IEnumerable<Note> notes)
    {
        return notes
            .Select(n => n.ToDto())
            .ToList();
    }
}

using System.Text.Json;

namespace JodWai.Application.Notes.Dtos;

public sealed record NoteLinkDto(
    Guid Id
);

public record NoteDto
(
    Guid Id,
    string Title,
    JsonElement Content,
    IReadOnlyCollection<NoteLinkDto> Links,
    IReadOnlyCollection<string> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

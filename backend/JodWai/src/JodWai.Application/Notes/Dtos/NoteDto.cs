using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Notes.Dtos;

public record NoteDto
(
    Guid Id,
    string Title,
    string Content,
    IReadOnlyCollection<NoteLink> Links,
    IReadOnlyCollection<Tag> Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

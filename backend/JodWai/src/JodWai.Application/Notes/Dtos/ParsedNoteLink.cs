namespace JodWai.Application.Notes.Dtos;

public sealed record ParsedNoteLink(
// example: "[[Test Note]]"
    string RawText,
// example: "Test Note"
    string TargetTitle
);

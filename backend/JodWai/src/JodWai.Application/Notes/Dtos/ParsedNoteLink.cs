namespace JodWai.Application.Notes.Dtos;

public sealed record ParsedNoteLink(
    string RawText,
    string TargetTitle
);

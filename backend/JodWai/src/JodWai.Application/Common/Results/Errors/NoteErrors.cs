namespace JodWai.Application.Common.Results.Errors;

public static class NoteErrors
{
    public const string NotFoundCode = "Notes.NotFound";
    public const string DuplicateTitleCode = "Notes.DuplicateTitle";

    public static Error NotFound(Guid id) =>
        new(
            NotFoundCode,
            $"Note '{id}' was not found.");

    public static Error DuplicateTitle(string title) =>
        new(
            DuplicateTitleCode,
            $"Note title '{title}' already exists.");
}

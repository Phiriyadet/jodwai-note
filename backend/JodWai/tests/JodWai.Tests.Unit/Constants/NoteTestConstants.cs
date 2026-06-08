namespace JodWai.Tests.Unit.Constants;

internal static class NoteTestConstants
{
    public const string ValidNoteTitle = "My Note";
    public const string ValidNoteContent = "# Title\n\nSome content";
    public static readonly Guid ValidNoteId =
        Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    public static readonly Guid AnotherValidNoteId =
        Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
}

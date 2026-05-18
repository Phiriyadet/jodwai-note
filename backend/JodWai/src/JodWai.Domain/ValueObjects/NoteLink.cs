using JodWai.Domain.ValueObjects;

public sealed record NoteLink
{
    public NoteId SourceId { get; init; }
    public NoteId TargetId { get; init; }
    public string? Label { get; init; }

    private NoteLink() { }

    private NoteLink(
        NoteId sourceId,
        NoteId targetId,
        string? label)
    {
        if (sourceId == targetId)
        {
            throw new ArgumentException(
                "A note cannot link to itself.");
        }

        SourceId = sourceId;
        TargetId = targetId;
        Label = label;
    }

    public static NoteLink Create(
        NoteId sourceId,
        NoteId targetId,
        string? label = null)
    {
        return new(
            sourceId,
            targetId,
            label);
    }
}

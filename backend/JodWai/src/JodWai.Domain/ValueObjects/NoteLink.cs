using JodWai.Domain.ValueObjects;

public sealed record NoteLink
{
    public NoteId TargetId { get; }

    private NoteLink() { }

    private NoteLink(NoteId targetId)
    {
        TargetId = targetId;
    }

    public static NoteLink Create(NoteId targetId, NoteId sourceId)
    {
        if (sourceId == targetId)
        {
            throw new ArgumentException(
                "A note cannot link to itself.");
        }
        
        return new NoteLink(targetId);
    }
}

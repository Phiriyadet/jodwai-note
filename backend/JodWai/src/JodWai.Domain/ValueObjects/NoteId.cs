namespace JodWai.Domain.ValueObjects;

public readonly record struct NoteId
{
    public Guid Value { get; }

    private NoteId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("NoteId cannot be empty.");
        Value = value;
    }

    public static NoteId New() => new(Guid.NewGuid());
    public static NoteId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}

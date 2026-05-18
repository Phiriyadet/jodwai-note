namespace JodWai.Domain.ValueObjects;

public sealed record NoteContent
{
    public const int MaxLength = 10000;
    public string Value { get; }

    public NoteContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Note content cannot be empty.");
        }

        if (value.Length > MaxLength)
        {
            throw new ArgumentException(
                "Note content is too long.");
        }

        Value = value;
    }

    public override string ToString() => Value;
}

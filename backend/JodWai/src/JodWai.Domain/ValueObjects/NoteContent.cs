namespace JodWai.Domain.ValueObjects;

public sealed record NoteContent
{
    public const int MaxLength = 10000;
    public string Value { get; }

    private NoteContent(string value)
    {
        Value = value;
    }

    public static NoteContent From(string value)
    {
        string settledValue = (value ?? string.Empty).Trim();

        if (settledValue.Length > MaxLength)
        {
            throw new ArgumentException("Note content is too long.");
        }

        return new NoteContent(settledValue);
    }

    public override string ToString() => Value;
}

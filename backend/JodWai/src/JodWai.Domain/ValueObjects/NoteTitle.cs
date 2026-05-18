namespace JodWai.Domain.ValueObjects;

public sealed record NoteTitle
{
    public const int MaxLength = 200;

    public string Value { get; }

    private NoteTitle(string value)
    {
        Value = value;
    }

    public static NoteTitle From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Title cannot be empty.");
        }

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Title cannot exceed {MaxLength} characters.");
        }

        return new NoteTitle(value);
    }

    public override string ToString() => Value;

}

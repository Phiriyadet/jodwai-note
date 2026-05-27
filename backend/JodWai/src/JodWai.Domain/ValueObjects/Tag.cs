namespace JodWai.Domain.ValueObjects;

public sealed record Tag
{
    public const int MaxLength = 100;

    public string Value { get; }

    private Tag(string value)
    {
        Value = value;
    }

    public static Tag From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Tag cannot be empty.");
        }

        value = value.Trim().ToLowerInvariant();

        if (value.Length > MaxLength)
        {
            throw new ArgumentException(
                "Tag cannot exceed {MaxLength} characters.");
        }

        return new Tag(value);
    }

    public override string ToString() => Value;
}

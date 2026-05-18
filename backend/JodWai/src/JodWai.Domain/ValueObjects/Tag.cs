namespace JodWai.Domain.ValueObjects;

public sealed record Tag
{
    public string Value { get; }

    public Tag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Tag cannot be empty.");
        }

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;
}

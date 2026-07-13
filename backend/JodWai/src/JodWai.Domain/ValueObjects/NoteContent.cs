namespace JodWai.Domain.ValueObjects;

public sealed record NoteContent
{
    const string EmptyContent =
    """
    {
      "type": "doc",
      "content": [
        {
          "type": "paragraph",
          "content": [
            {
              "type": "text",
              "text": ""
            }
          ]
        }
      ]
    }
    """;
    public const int MaxLength = 10000;
    public string Value { get; }

    private NoteContent(string value)
    {
        Value = value;
    }

    public static NoteContent Empty => new(EmptyContent);

    public static NoteContent From(string? value)
    {
        var settledValue = string.IsNullOrWhiteSpace(value)
            ? EmptyContent
            : value.Trim();

        if (settledValue.Length > MaxLength)
        {
            throw new ArgumentException("Note content is too long.");
        }

        return new NoteContent(settledValue);
    }

    public override string ToString() => Value;
}

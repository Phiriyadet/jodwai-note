using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

namespace JodWai.Tests.Unit.Domain.Shared;

public class NoteBuilder
{
    private Note? _note;
    private NoteTitle? _title;
    private NoteContent? _content;
    private bool _initialized = false;

    public static NoteBuilder NewNote() => new();

    public NoteBuilder WithTitle(string title)
    {
        _title = NoteTitle.From(title);
        return this;
    }

    public NoteBuilder WithContent(string content)
    {
        _content = NoteContent.From(content);
        return this;
    }

    public NoteBuilder WithMaxLenContent()
    {
        var maxLength = NoteContent.MaxLength;
        _content = NoteContent.From(new string('A', maxLength));
        return this;
    }

    public NoteBuilder WithNullTitle()
    {
        _title = null!;
        return this;
    }

    public NoteBuilder WithNullContent()
    {
        _content = null!;
        return this;
    }

    public NoteBuilder WithEmptyContent()
    {
        _content = NoteContent.From(string.Empty);
        return this;
    }

    public Note Build()
    {
        if (!_initialized)
        {
            _note = Note.Create(
                _title ?? throw new ArgumentNullException(nameof(_title)),
                _content ?? throw new ArgumentNullException(nameof(_content))
            );
            _initialized = true;
        }
        return _note;
    }

    public Note BuildValid()
    {
        if (!_initialized)
        {
            _note = Note.Create(
                _title ?? NoteTitle.From("Default Title"),
                _content ?? NoteContent.From("Default content")
            );
            _initialized = true;
        }
        return _note;
    }
}

using JodWai.Domain.ValueObjects;

namespace JodWai.Domain.Entities;

public class Note
{
    private readonly List<NoteLink> _links = [];
    private readonly List<Tag> _tags = [];

    public NoteId Id { get; }
    public NoteTitle Title { get; private set; }
    public NoteContent Content { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyList<NoteLink> Links => _links;
    public IReadOnlyCollection<Tag> Tags => _tags;

    private Note()
    {
        Title = null!;
        Content = null!;
    } // for ORM

    private Note(NoteId id, NoteTitle title, NoteContent content)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static Note Create(NoteTitle title, NoteContent content)
       => new(NoteId.New(), title, content);

    public void Update(
        NoteTitle? title = null,
        NoteContent? content = null)
    {
        if (title is null && content is null)
        {
            throw new InvalidOperationException(
                "At least one field must be updated.");
        }

        if (title is not null)
        {
            Title = title;
        }

        if (content is not null)
        {
            Content = content;
        }
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void AddLink(NoteId targetId)
    {
        if (_links.Any(l => l.TargetId == targetId))
            return;

        _links.Add(NoteLink.Create(targetId, Id));
    }

    private void RemoveLink(NoteId targetId)
    {
        _links.RemoveAll(x => x.TargetId == targetId);
    }

    public void SyncLinks(List<NoteId> validIds)
    {
        if (!validIds.Any())
        {
            _links.Clear();
            return;
        }

        var validSet = validIds.ToHashSet();

        var orphanedIds = _links
            .Where(l => !validSet.Contains(l.TargetId))
            .Select(l => l.TargetId)
            .ToList();

        foreach (var id in orphanedIds)
            RemoveLink(id);

        foreach (var targetId in validSet)
            AddLink(targetId);
    }

    public void AddTag(Tag tag) => _tags.Add(tag);
    public void RemoveTag(Tag tag) => _tags.Remove(tag);

}

using JodWai.Domain.ValueObjects;
using JodWai.Tests.Unit.Domain.Shared;

namespace JodWai.Tests.Unit.Domain.Entities;

public class NoteTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsNoteWithGeneratedIdAndTimestamp()
    {
        // Arrange
        var sut = new NoteBuilder()
            .WithTitle("My Note")
            .WithContent("Some content here")
            .Build();

        // Assert
        sut.Id.Should().NotBe(Guid.Empty);
        sut.Title.Value.Should().Be("My Note");
        sut.Content.Value.Should().Be("Some content here");
        sut.CreatedAt.Should().NotBe(DateTimeOffset.MinValue);
        sut.UpdatedAt.Should().Be(sut.CreatedAt);
    }

    [Fact]
    public void Create_WithNullTitle_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new NoteBuilder()
            .WithNullTitle()
            .WithContent("Content");

        // Act & Assert
        Action act = () => sut.Build();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullContent_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new NoteBuilder()
            .WithTitle("My Note")
            .WithNullContent();

        // Act & Assert
        Action act = () => sut.Build();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithExactMaxLengthContent_Succeeds()
    {
        // Arrange
        var sut = new NoteBuilder()
            .WithTitle("My Title")
            .WithMaxLenContent()
            .Build();

        // Assert
        sut.Content.Value.Length.Should().Be(NoteContent.MaxLength);
    }

    [Fact]
    public void Update_WithNoParameters_ThrowsInvalidOperationException()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        // Act & Assert
        Action act = () => sut.Update(null!, null!);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*At least one field must be updated*");
    }

    [Fact]
    public void Update_WithTitleOnly_UpdatesTitleAndTimestamp()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var newTitle = NoteTitle.From("New Title");

        // Act
        sut.Update(title: newTitle);

        // Assert
        sut.Title.Value.Should().Be("New Title");
        sut.UpdatedAt.Should().NotBe(sut.CreatedAt);
    }

    [Fact]
    public void Update_WithContentOnly_UpdatesContentAndTimestamp()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var newContent = NoteContent.From("New Content");

        // Act
        sut.Update(content: newContent);

        // Assert
        sut.Content.Value.Should().Be("New Content");
        sut.UpdatedAt.Should().NotBe(sut.CreatedAt);
    }

    [Fact]
    public void Update_WithBothParameters_UpdatesBothAndTimestamp()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var newTitle = NoteTitle.From("New Title");
        var newContent = NoteContent.From("New Content");

        // Act
        sut.Update(title: newTitle, content: newContent);

        // Assert
        sut.Title.Value.Should().Be("New Title");
        sut.Content.Value.Should().Be("New Content");
        sut.UpdatedAt.Should().NotBe(sut.CreatedAt);
    }

    [Fact]
    public void Update_WithExistingTitleValue_UpdatesWithoutChange()
    {
        // Arrange
        var title = "Original Title";
        var sut = new NoteBuilder()
            .WithTitle(title)
            .BuildValid();

        var sameTitle = NoteTitle.From(title);

        // Act
        sut.Update(title: sameTitle);

        // Assert
        sut.Title.Value.Should().Be(title);
    }

    [Fact]
    public void SyncLinks_WithEmptyList_ClearsAllLinks()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();
        var link1 = NoteId.New();
        var link2 = NoteId.New();

        sut.SyncLinks(new List<NoteId> { link1, link2 });

        // Act
        sut.SyncLinks(new List<NoteId>());

        // Assert
        sut.Links.Should().BeEmpty();
    }

    [Fact]
    public void SyncLinks_WithSelfReference_ThrowsArgumentException()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        // Act & Assert
        Action act = () => sut.SyncLinks(new List<NoteId> { sut.Id });
        act.Should().Throw<ArgumentException>()
            .WithMessage("*A note cannot link to itself*");
    }

    [Fact]
    public void SyncLinks_WithOrphanedLinks_RemovesOrphansAndAddsNew()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var target1 = NoteId.New();
        var target2 = NoteId.New();
        var orphanId = NoteId.New();
        var newTarget = NoteId.New();

        sut.SyncLinks(new List<NoteId> { target1, target2, orphanId });

        // Act
        sut.SyncLinks(new List<NoteId> { target1, newTarget });

        // Assert
        sut.Links.Should().HaveCount(2);
    }

    [Fact]
    public void SyncLinks_WithDuplicateTargetIds_AddsSingleLink()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var targetId = NoteId.New();

        // Act
        sut.SyncLinks(new List<NoteId> { targetId, targetId });

        // Assert
        sut.Links.Should().HaveCount(1);
    }

    [Fact]
    public void SyncLinks_WithFirstSyncAddsLinksAndSecondSyncClearsThem()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var firstSyncTargets = new List<NoteId> { NoteId.New(), NoteId.New() };

        // Act
        sut.SyncLinks(firstSyncTargets);
        sut.SyncLinks(new List<NoteId>());

        // Assert
        sut.Links.Should().BeEmpty();
    }

    [Fact]
    public void AddTag_AddsTagTrimmedAndLowercased()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();
        var whitespaceTag = Tag.From("  Important ");

        // Act
        sut.AddTag(whitespaceTag);

        // Assert
        sut.Tags.Should().ContainSingle();
    }

    [Fact]
    public void AddTag_WithDuplicateTagValue_AddsWithoutDeduplication()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();
        var tag1 = Tag.From("important");
        var tag2 = Tag.From("important");

        // Act
        sut.AddTag(tag1);
        sut.AddTag(tag2);

        // Assert
        sut.Tags.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveTag_RemovesTagByValue()
    {
        // Arrange
        var sut = new NoteBuilder()
            .BuildValid();

        var tag1 = Tag.From("important");
        var tag2 = Tag.From("urgent");

        sut.AddTag(tag1);
        sut.AddTag(tag2);

        // Act
        sut.RemoveTag(tag1);

        // Assert
        sut.Tags.Should().HaveCount(1);
    }
}

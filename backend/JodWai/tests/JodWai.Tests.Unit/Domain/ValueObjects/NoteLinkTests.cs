using JodWai.Domain.ValueObjects;

namespace JodWai.Tests.Unit.Domain.ValueObjects;

public class NoteLinkTests
{
   [Fact]
    public void Create_WithDifferentSourceAndTarget_ReturnsLinkWithAllProperties()
    {
        // Arrange
        var sourceId = NoteId.New();
        var targetId = NoteId.New();

        // Act
        var sut = NoteLink.Create(targetId, sourceId);

        // Assert
        sut.TargetId.Should().Be(targetId);
    }

    // ==============================
    // 2. Failure Paths
    // ==============================

    [Fact]
    public void Create_WithSameSourceAndTarget_ThrowsArgumentException()
    {
        // Arrange
        var id = NoteId.New();

        // Act
        Action act = () => NoteLink.Create(id, id);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*A note cannot link to itself*");
    }
}

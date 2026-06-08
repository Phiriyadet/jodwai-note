using JodWai.Domain.ValueObjects;

namespace JodWai.Tests.Unit.Domain.ValueObjects;

public class NoteIdTests
{
    // ==============================
    // 1. Happy Path
    // ==============================

    [Fact]
    public void From_WithValidGuid_ReturnsNoteIdWithValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var sut = NoteId.From(guid);

        // Assert
        sut.Value.Should().Be(guid);
    }

    [Fact]
    public void ToString_ReturnsGuidAsString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var sut = NoteId.From(guid);

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void New_ReturnsNoteIdWithNonEmptyGuid()
    {
        // Act
        var sut = NoteId.New();

        // Assert
        sut.Value.Should().NotBe(Guid.Empty);
    }

    // ==============================
    // 2. Failure Paths
    // ==============================

    [Fact]
    public void From_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var act = () => NoteId.From(Guid.Empty);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be empty*");
    }
}

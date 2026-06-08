using JodWai.Domain.ValueObjects;

namespace JodWai.Tests.Unit.Domain.ValueObjects;

public class NoteContentTests
{
    // ==============================
    // 1. Happy Path
    // ==============================

    [Fact]
    public void From_WithValidContent_ReturnsNoteContent()
    {
        // Arrange
        var content = "This is some valid note content";

        // Act
        var sut = NoteContent.From(content);

        // Assert
        sut.Value.Should().Be(content);
    }

    [Fact]
    public void ToString_ReturnsContentValue()
    {
        // Arrange
        var content = "Hello world";
        var sut = NoteContent.From(content);

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public void From_WithNullContent_TreatsAsEmptyString()
    {
        // Arrange
        var sut = NoteContent.From(null);

        // Act
        var result = sut.Value;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void From_WithWhitespaceOnly_TrimsAndReturnsEmpty()
    {
        // Arrange
        var content = "   ";

        // Act
        var sut = NoteContent.From(content);

        // Assert
        sut.Value.Should().BeEmpty();
    }

    // ==============================
    // 2. Failure Paths
    // ==============================

    [Fact]
    public void From_WithContentExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var act = () => NoteContent.From(new string('a', NoteContent.MaxLength + 1));

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Note content is too long.");
    }
}

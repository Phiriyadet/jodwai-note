using JodWai.Domain.ValueObjects;

namespace JodWai.Tests.Unit.Domain.ValueObjects;

public class NoteTitleTests
{
    // ==============================
    // 1. Happy Path
    // ==============================

    [Fact]
    public void From_WithValidTitle_ReturnsNoteTitleWithValue()
    {
        // Arrange
        var title = "My Note Title";

        // Act
        var sut = NoteTitle.From(title);

        // Assert
        sut.Value.Should().Be(title);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var title = "Hello World";
        var sut = NoteTitle.From(title);

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().Be(title);
    }

    [Fact]
    public void From_WithWhitespaceOnly_TrimsAndReturnsCleanTitle()
    {
        // Arrange
        var title = "   Hello   ";

        // Act
        var sut = NoteTitle.From(title);

        // Assert
        sut.Value.Should().Be("Hello");
    }

    // ==============================
    // 2. Failure Paths
    // ==============================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyOrNullInput_ThrowsArgumentException(string input)
    {
        // Arrange
        var act = () => NoteTitle.From(input);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void From_WithTitleExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var act = () => NoteTitle.From(new string('a', NoteTitle.MaxLength + 1));

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Title cannot exceed {NoteTitle.MaxLength} characters.");
    }
}

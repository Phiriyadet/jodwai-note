using JodWai.Domain.ValueObjects;

namespace JodWai.Tests.Unit.Domain.ValueObjects;

public class TagTests
{
    // ==============================
    // 1. Happy Path
    // ==============================

    [Fact]
    public void From_WithValidName_ReturnsTrimmedLowercasedTag()
    {
        // Arrange
        var input = "  MyTag  ";

        // Act
        var sut = Tag.From(input);

        // Assert
        sut.Value.Should().Be("mytag");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var sut = Tag.From("testing");

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().Be("testing");
    }

    // ==============================
    // 2. Failure Paths
    // ==============================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyOrWhitespaceInput_ThrowsArgumentException(string input)
    {
        // Arrange
        var act = () => Tag.From(input);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tag cannot be empty*");
    }

     [Fact]
    public void From_WithTagExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Tag.From(new string('a', Tag.MaxLength + 1));

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"Tag cannot exceed {Tag.MaxLength} characters.");
    }
}

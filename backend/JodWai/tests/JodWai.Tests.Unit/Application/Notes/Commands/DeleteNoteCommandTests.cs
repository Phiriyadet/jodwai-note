using JodWai.Application.Interfaces;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using JodWai.Application.Notes.Commands;
using JodWai.Tests.Unit.Constants;

using Moq;

namespace JodWai.Tests.Unit.Application.Notes.Commands;

public class DeleteNoteCommandTests
{
    private readonly Mock<INoteRepository> _mockRepository = new();
    private readonly DeleteNoteCommandHandler sut;

    public DeleteNoteCommandTests()
    {
        sut = new DeleteNoteCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public void Handle_WithValidId_DeletesNoteSuccessfully()
    {
        // Arrange
        var command = new DeleteNoteCommand(NoteTestConstants.ValidNoteId);

        var note = Note.Create(
            NoteTitle.From(NoteTestConstants.ValidNoteTitle),
            NoteContent.From(NoteTestConstants.ValidNoteContent));

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(note);

        _mockRepository
            .Setup(r => r.Delete(It.IsAny<Note>()))
            .Verifiable();

        _mockRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(
            r => r.Delete(It.IsAny<Note>()), Times.Once);
        _mockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Handle_WhenNoteNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteNoteCommand(Guid.NewGuid());

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note?)null);

        // Act
        Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Note*not found*");
    }

    [Fact]
    public void Handle_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var command = new DeleteNoteCommand(Guid.NewGuid());

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Database*error*");
    }
}

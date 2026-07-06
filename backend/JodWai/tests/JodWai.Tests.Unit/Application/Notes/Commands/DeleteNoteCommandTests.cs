using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Interfaces;
using JodWai.Application.Notes.Commands.DeleteNote;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;
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
    public async Task Handle_WithValidId_DeletesNoteSuccessfully()
    {
        // Arrange: valid note id from constant, not used for repository call since handler converts to NoteId internally;
        var command = new DeleteNoteCommand(NoteTestConstants.ValidNoteId);

        var note = Note.Create(
            NoteTitle.From(NoteTestConstants.ValidNoteTitle),
            NoteContent.From(NoteTestConstants.ValidNoteContent.GetRawText()));

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(note);

        _mockRepository.Setup(r => r.Delete(It.IsAny<Note>())).Verifiable();

        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act: handler retrieves note, deletes it via repository method call then persists state change
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert: success returned with all expected repository methods called exactly once each in order
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Note>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Test cleanup to avoid VerifyMultiple failures when tests run sequentially
    }

    [Fact]
    public async Task Handle_WhenNoteNotFound_ReturnsNotFoundError()
    {
        // Arrange: command with fresh id; repository returns null since note doesn't exist in database yet.
        var command = new DeleteNoteCommand(Guid.NewGuid());

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note?)null);

        // Act: handler detects missing entity and returns failure result without invoking SaveChanges or delete.
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert: verification that repository was called exactly once to check for note existence before returning NotFound error
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NoteErrors.NotFound(command.Id));
        
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_RejectsExceptionToCaller()
    {
        // Arrange: repository has database-level error during retrieval phase; handler must propagate.
        var command = new DeleteNoteCommand(Guid.NewGuid());

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act: unhandled exceptions from repository bubble up without being caught or wrapped.
        Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

        // Assert: caller receives the underlying exception and message fragment for debugging purposes
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Database*connection failed");
    }
}

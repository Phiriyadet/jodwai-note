using JodWai.Application.Interfaces;
using JodWai.Application.Notes.Commands;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;
using JodWai.Tests.Unit.Constants;

using Moq;

namespace JodWai.Tests.Unit.Application.Notes.Commands;

public class CreateNoteCommandTests
{
    private readonly Mock<INoteRepository> _mockRepository = new();
    private readonly Mock<INoteLinkParser> _mockParser = new();
    private readonly CreateNoteCommandHandler _sut;

    public CreateNoteCommandTests()
    {
        _sut = new(_mockRepository.Object, _mockParser.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTitle_ThrowsInvalidOperationExceptionBeforePersisting()
    {
        // Arrange
        var request = new CreateNoteRequest(NoteTestConstants.ValidNoteTitle, NoteTestConstants.ValidNoteContent);
        var command = new CreateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(NoteTestConstants.ValidNoteTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.Handle(command, CancellationToken.None));

        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAllLinksResolveToExistingNotes_PersistsNoteWithLinkedIds()
    {
        // Arrange
        var request = new CreateNoteRequest(NoteTestConstants.ValidNoteTitle, NoteTestConstants.ValidNoteContent);
        var command = new CreateNoteCommand(request);

        var linkedNoteTitle = "Existing Linked Note";
        var linkedNoteId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-fed234567890");

        var parsedLinks = new List<ParsedNoteLink>
        {
            new ParsedNoteLink(RawText: $"[[{linkedNoteTitle}]]", TargetTitle: linkedNoteTitle)
        };

        // Title check for the new note itself — does not exist yet
        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(NoteTestConstants.ValidNoteTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        _mockParser
            .Setup(p => p.Parse(It.IsAny<NoteContent>()))
            .Returns(parsedLinks);

        // Link target resolves to an existing note
        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(linkedNoteTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(linkedNoteId);

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note note, CancellationToken _) => note);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Blank note should NOT have been created for the resolved link
        _mockRepository.Verify(r => r.CreateAsync(
            It.Is<Note>(n => n.Title.Value == linkedNoteTitle),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenLinkTargetsMissing_CreatesBlankNotesForEachMissingTarget()
    {
        // Arrange
        var request = new CreateNoteRequest(NoteTestConstants.ValidNoteTitle, NoteTestConstants.ValidNoteContent);
        var command = new CreateNoteCommand(request);

        var missingLinkTitle = "Missing Linked Note";

        var parsedLinks = new List<ParsedNoteLink>
        {
            new ParsedNoteLink(RawText: $"[[{missingLinkTitle}]]", TargetTitle: missingLinkTitle)
        };

        _mockParser
            .Setup(p => p.Parse(It.IsAny<NoteContent>()))
            .Returns(parsedLinks);

        // Both the new note title and the missing link resolve to null (don't exist)
        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note note, CancellationToken _) => note);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        // One blank note for the missing link + one for the note itself = 2 total
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        // Blank note created with the missing link's title and empty content
        _mockRepository.Verify(r => r.CreateAsync(
            It.Is<Note>(n => n.Title.Value == missingLinkTitle && n.Content.Value == ""),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

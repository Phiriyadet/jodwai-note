using System.Text.Json;

using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Interfaces;
using JodWai.Application.Notes.Commands.UpdateNote;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;
using JodWai.Tests.Unit.Constants;

using Moq;

namespace JodWai.Tests.Unit.Application.Notes.Commands;

public class UpdateNoteCommandHandlerTests
{
    private readonly Mock<INoteRepository> _mockRepository = new();
    private readonly Mock<INoteLinkResolver> _mockResolver = new();
    private readonly UpdateNoteCommandHandler _sut;

    public UpdateNoteCommandHandlerTests()
    {
        _sut = new(_mockRepository.Object, _mockResolver.Object);
    }

    [Fact]
    public async Task Handle_WhenNoteDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, NoteTestConstants.ValidNoteTitle, NoteTestConstants.ValidNoteContent);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note?)null);

        // Act & Assert
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NoteErrors.NotFound(request.Id));

        _mockRepository.Verify(r => r.Update(It.IsAny<Note>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNewTitleAlreadyExistsOnAnotherNote_ReturnError()
    {
        // Arrange
        var existingNote = Note.Create(
            title: NoteTitle.From(NoteTestConstants.ValidNoteTitle),
            content: NoteContent.From(NoteTestConstants.ValidNoteContent.GetRawText()));

        var conflictingNoteId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        const string newTitle = "A Different Title";

        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, newTitle, NoteTestConstants.ValidNoteContent);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNote);

        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(newTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conflictingNoteId);

        // Act & Assert
        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(NoteErrors.DuplicateTitleCode);

        _mockRepository.Verify(r => r.Update(It.IsAny<Note>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTitleUnchangedAndNoLinks_UpdatesNoteAndPersists()
    {
        // Arrange
        var existingNote = Note.Create(
            title: NoteTitle.From(NoteTestConstants.ValidNoteTitle),
            content: NoteContent.From("old content"));

        // Same title as existing note — no title conflict check triggered
        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, NoteTestConstants.ValidNoteTitle, NoteTestConstants.ValidNoteContent);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNote);

        //_mockParser
        //    .Setup(p => p.Parse(It.IsAny<NoteContent>()))
        //    .Returns(new List<ParsedNoteLink>());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.Update(existingNote), Times.Once);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(r => r.GetNotesReferencingAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTitleChangedAndNoReferencingNotes_UpdatesNoteAndPersists()
    {
        // Arrange
        const string oldTitle = "Old Title";
        const string newTitle = "New Title";

        var existingNote = Note.Create(
            title: NoteTitle.From(oldTitle),
            content: NoteContent.From(NoteTestConstants.ValidNoteContent.GetRawText()));

        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, newTitle, NoteTestConstants.ValidNoteContent);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNote);

        // New title is available
        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(newTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        //_mockParser
        //    .Setup(p => p.Parse(It.IsAny<NoteContent>()))
        //    .Returns(new List<ParsedNoteLink>());

        // No notes reference this note
        _mockRepository
            .Setup(r => r.GetNotesReferencingAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Note>());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.Update(existingNote), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTitleChangedAndReferencingNotesExist_UpdatesLinkTextInEachReferencingNote()
    {
        // Arrange
        const string oldTitle = "Old Title";
        const string newTitle = "New Title";

        var existingNote = Note.Create(
            title: NoteTitle.From(oldTitle),
            content: NoteContent.From(NoteTestConstants.ValidNoteContent.GetRawText()));

        // Two notes that contain a link to the renamed note
        var referencingNote1 = Note.Create(
            title: NoteTitle.From("Referencing Note 1"),
            content: NoteContent.From($"See [[{oldTitle}]] for details."));

        var referencingNote2 = Note.Create(
            title: NoteTitle.From("Referencing Note 2"),
            content: NoteContent.From($"Also see [[{oldTitle}]]."));

        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, newTitle, NoteTestConstants.ValidNoteContent);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNote);

        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(newTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        //_mockParser
        //    .Setup(p => p.Parse(It.IsAny<NoteContent>()))
        //    .Returns(new List<ParsedNoteLink>());

        _mockRepository
            .Setup(r => r.GetNotesReferencingAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Note> { referencingNote1, referencingNote2 });

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        // The renamed note itself + 2 referencing notes = 3 Update calls total
        _mockRepository.Verify(r => r.Update(It.IsAny<Note>()), Times.Exactly(3));

        // Each referencing note should have had its link text rewritten
        referencingNote1.Content.Value.Should().Contain($"[[{newTitle}]]");
        referencingNote1.Content.Value.Should().NotContain($"[[{oldTitle}]]");

        referencingNote2.Content.Value.Should().Contain($"[[{newTitle}]]");
        referencingNote2.Content.Value.Should().NotContain($"[[{oldTitle}]]");

        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAllLinkTargetsExist_DoesNotCreateAnyBlankNotes()
    {
        // Arrange
        var existingNote = Note.Create(
            title: NoteTitle.From(NoteTestConstants.ValidNoteTitle),
            content: NoteContent.From("old content"));

        const string linkedTitle = "Linked Note";
        var linkedNoteId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, NoteTestConstants.ValidNoteTitle, JsonDocument.Parse($"\"See [[{linkedTitle}]].\"").RootElement);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNote);

        //_mockParser
        //    .Setup(p => p.Parse(It.IsAny<NoteContent>()))
        //    .Returns(new List<ParsedNoteLink>
        //    {
        //        new(RawText: $"[[{linkedTitle}]]", TargetTitle: linkedTitle)
        //    });

        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(linkedTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(linkedNoteId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenLinkTargetDoesNotExist_CreatesBlankNoteForMissingTarget()
    {
        // Arrange
        var existingNote = Note.Create(
            title: NoteTitle.From(NoteTestConstants.ValidNoteTitle),
            content: NoteContent.From("old content"));

        const string missingTitle = "Missing Note";

        var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, NoteTestConstants.ValidNoteTitle, JsonDocument.Parse($"\"See [[{missingTitle}]].\"").RootElement);
        var command = new UpdateNoteCommand(request);

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNote);

        //_mockParser
        //    .Setup(p => p.Parse(It.IsAny<NoteContent>()))
        //    .Returns(new List<ParsedNoteLink>
        //    {
        //        new(RawText: $"[[{missingTitle}]]", TargetTitle: missingTitle)
        //    });

        // The missing link title resolves to null
        _mockRepository
            .Setup(r => r.GetIdByTitleAsync(missingTitle, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Note note, CancellationToken _) => note);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        // One blank note created for the missing link
        _mockRepository.Verify(r => r.CreateAsync(
            It.Is<Note>(n => n.Title.Value == missingTitle && n.Content.Value == string.Empty),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //[Fact]
    //public async Task Handle_WhenTitleAndContentAreEmptyStrings_TreatsThemAsNoChange()
    //{
    //    // Arrange
    //    var existingNote = Note.Create(
    //        title: NoteTitle.From(NoteTestConstants.ValidNoteTitle),
    //        content: NoteContent.From(NoteTestConstants.ValidNoteContent.GetRawText()));

    //    // Empty strings → handler treats them as null (no-change)
    //    var request = new UpdateNoteRequest(NoteTestConstants.ValidNoteId, string.Empty, NoteTestConstants.EmptyNoteContent);
    //    var command = new UpdateNoteCommand(request);

    //    _mockRepository
    //        .Setup(r => r.GetByIdAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(existingNote);

    //    // parser is never called because content is null after the guard
    //    // no GetIdByTitleAsync because title didn't change

    //    // Act
    //    var result = await _sut.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.Value.Title.Should().Be(existingNote.Title.Value);
    //    result.Value.Content.Should().Be(existingNote.Content.Value);
    //    _mockParser.Verify(p => p.Parse(It.IsAny<NoteContent>()), Times.Never);
    //    _mockRepository.Verify(r => r.GetIdByTitleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    //    _mockRepository.Verify(r => r.GetNotesReferencingAsync(It.IsAny<NoteId>(), It.IsAny<CancellationToken>()), Times.Never);
    //    _mockRepository.Verify(r => r.Update(existingNote), Times.Never);
    //    _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    //}
}

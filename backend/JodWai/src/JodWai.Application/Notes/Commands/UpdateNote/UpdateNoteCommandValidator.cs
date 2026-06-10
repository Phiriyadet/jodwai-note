using FluentValidation;

using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Notes.Commands.UpdateNote;

public sealed class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Request.Id)
            .NotEmpty()
            .WithMessage("Note ID is required.");
        RuleFor(x => x.Request.Title)
            .MaximumLength(NoteTitle.MaxLength)
            .WithMessage($"Title must not exceed {NoteTitle.MaxLength} characters.");
        RuleFor(x => x.Request.Content)
            .MaximumLength(NoteContent.MaxLength)
            .WithMessage($"Content must not exceed {NoteContent.MaxLength} characters.");
    }
}

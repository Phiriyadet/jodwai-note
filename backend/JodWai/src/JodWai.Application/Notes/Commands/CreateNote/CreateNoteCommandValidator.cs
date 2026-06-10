using FluentValidation;

using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Notes.Commands.CreateNote;

public sealed class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(NoteTitle.MaxLength)
            .WithMessage($"Title must not exceed {NoteTitle.MaxLength} characters.");
        RuleFor(x => x.Request.Content)
            .MaximumLength(NoteContent.MaxLength)
            .WithMessage($"Content must not exceed {NoteContent.MaxLength} characters.");
    }
}

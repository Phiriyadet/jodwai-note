using FluentValidation;

namespace JodWai.Application.Notes.Queries.GetNotes;

public sealed class GetNotesQueryValidator : AbstractValidator<GetNotesQuery>
{
    public GetNotesQueryValidator()
    {
        RuleFor(x => x.Request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.Request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Request)
            .Must(request =>
                request.CreatedAfter is null ||
                request.CreatedBefore is null ||
                request.CreatedAfter <= request.CreatedBefore)
            .WithMessage("CreatedAfter must be less than or equal to CreatedBefore.");

        RuleFor(x => x.Request.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sortBy.");

        RuleFor(x => x.Request.SortOrder)
            .IsInEnum()
            .WithMessage("Invalid sortOrder.");
    }
}

using JodWai.Application.Common.Enums;
using JodWai.Application.Notes.Dtos.Enums;

namespace JodWai.Application.Common.Pagination;

public sealed record GetNotesOptions(
    int Page,
    int PageSize,
    string? Search,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    NoteSortBy SortBy,
    SortOrder SortOrder);

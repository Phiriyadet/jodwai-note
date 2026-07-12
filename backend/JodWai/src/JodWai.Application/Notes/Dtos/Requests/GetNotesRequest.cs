using JodWai.Application.Common.Enums;
using JodWai.Application.Notes.Dtos.Enums;

namespace JodWai.Application.Notes.Dtos.Requests;

public sealed record GetNotesRequest
(
    int Page = 1,
    int PageSize = 20,

    string? Search = null,
    string? Tag = null,

    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null,

    NoteSortBy SortBy = NoteSortBy.UpdatedAt,
    SortOrder SortOrder = SortOrder.Desc
);

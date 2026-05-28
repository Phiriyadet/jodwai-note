namespace JodWai.Application.Notes.Dtos.Requests;

public record UpdateNoteRequest
(Guid Id, string Title, string Content);

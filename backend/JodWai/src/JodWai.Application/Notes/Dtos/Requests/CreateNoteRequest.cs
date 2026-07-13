using System.Text.Json;

namespace JodWai.Application.Notes.Dtos.Requests;

public record CreateNoteRequest
(string Title, JsonElement Content);

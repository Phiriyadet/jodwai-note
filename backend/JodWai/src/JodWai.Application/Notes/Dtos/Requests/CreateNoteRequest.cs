using System.Text.Json;

using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Notes.Dtos.Requests;

public record CreateNoteRequest
(string Title, JsonElement Content);

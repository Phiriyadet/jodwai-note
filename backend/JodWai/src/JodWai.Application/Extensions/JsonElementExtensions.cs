using System.Text.Json;

using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Extensions;

public static class JsonElementExtensions
{
    public static string ToNoteContentJson(this JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null => NoteContent.Empty.ToString(),
            JsonValueKind.Undefined => NoteContent.Empty.ToString(),
            _ => element.GetRawText()
        };
    }
}

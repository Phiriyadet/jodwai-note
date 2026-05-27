using JodWai.Application.Notes.Dtos;
using JodWai.Domain.ValueObjects;

namespace JodWai.Application.Interfaces;

public interface INoteLinkParser
{
    IReadOnlyCollection<ParsedNoteLink> Parse(NoteContent content);
}

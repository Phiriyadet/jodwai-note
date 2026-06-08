using System.Text.RegularExpressions;

using JodWai.Application.Interfaces;
using JodWai.Application.Notes.Dtos;
using JodWai.Domain.ValueObjects;

namespace JodWai.Infrastructure.Parsing;

public sealed class WikiStyleNoteLinkParser : INoteLinkParser
{
    private static readonly Regex LinkRegex =
        new(@"\[\[(.+?)\]\]",
            RegexOptions.Compiled);

    public IReadOnlyCollection<ParsedNoteLink> Parse(NoteContent content)
    {
        if (string.IsNullOrWhiteSpace(content.Value))
        {
            return [];
        }

        var results = new List<ParsedNoteLink>();

        var matches = LinkRegex.Matches(content.Value)
            .Cast<Match>()
            .GroupBy(m => m.Value)
            .Select(g => g.First())
            .ToList();

        if (matches == null || !matches.Any()) return results;

        foreach (var match in matches)
        {

            var target = match.Groups[1].Value.Trim();

            if (string.IsNullOrEmpty(target))
            {
                continue;
            }

            results.Add(new ParsedNoteLink(match.Value, target));
        }

        return results;
    }
}

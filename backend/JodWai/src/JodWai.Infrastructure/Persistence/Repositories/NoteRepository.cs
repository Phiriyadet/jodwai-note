using JodWai.Application.Common.Enums;
using JodWai.Application.Common.Pagination;
using JodWai.Application.Interfaces;
using JodWai.Application.Notes.Dtos.Enums;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace JodWai.Infrastructure.Persistence.Repositories;

internal class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context;
    public NoteRepository(AppDbContext context) => _context = context;

    public async Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default)
        => (await _context.Notes
        .AddAsync(note, cancellationToken)).Entity;

    public async Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default)
        => await _context.Notes
        .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<Guid?> GetIdByTitleAsync(string title, CancellationToken cancellationToken = default)
        => await _context.Notes
        .Where(n => n.Title.Value == title)
        .Select(n => (Guid?)n.Id.Value)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Note>> GetNotesReferencingAsync(NoteId noteId, CancellationToken cancellationToken)
        => await _context.Notes
        .Where(n => n.Links
        .Any(l => l.TargetId == noteId))
        .ToListAsync(cancellationToken);

    public async Task<PagedResult<Note>> GetPagedAsync(
    GetNotesOptions options,
    CancellationToken cancellationToken = default)
    {
        IQueryable<Note> query = _context.Notes;

        DateTimeOffset? createdAfter = null;
        DateTimeOffset? createdBefore = null;

        if (options.CreatedAfter is not null)
        {
            createdAfter = new DateTimeOffset(
                options.CreatedAfter.Value.ToDateTime(TimeOnly.MinValue),
                TimeSpan.Zero);
        }

        if (options.CreatedBefore is not null)
        {
            createdBefore = new DateTimeOffset(
                options.CreatedBefore.Value.ToDateTime(TimeOnly.MaxValue),
                TimeSpan.Zero);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            query = query.Where(x =>
                EF.Functions.ILike(x.Title.Value, $"%{options.Search}%"));
        }

        // Created date filter
        if (createdAfter is not null)
        {
            query = query.Where(x =>
                x.CreatedAt >= createdAfter.Value);
        }

        if (createdBefore is not null)
        {
            query = query.Where(x =>
                x.CreatedAt <= createdBefore.Value);
        }

        // Sorting
        query = (options.SortBy, options.SortOrder) switch
        {
            (NoteSortBy.Title, SortOrder.Asc) =>
                query.OrderBy(x => x.Title.Value),

            (NoteSortBy.Title, SortOrder.Desc) =>
                query.OrderByDescending(x => x.Title.Value),

            (NoteSortBy.CreatedAt, SortOrder.Asc) =>
                query.OrderBy(x => x.CreatedAt),

            (NoteSortBy.CreatedAt, SortOrder.Desc) =>
                query.OrderByDescending(x => x.CreatedAt),

            (NoteSortBy.UpdatedAt, SortOrder.Asc) =>
                query.OrderBy(x => x.UpdatedAt),

            _ =>
                query.OrderByDescending(x => x.UpdatedAt)
        };

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Note>(
            Items: items,
            Page: options.Page,
            PageSize: options.PageSize,
            TotalItems: totalItems);
    }

    public void Update(Note note)
        => _context.Notes.Update(note);
    public void Delete(Note note)
        => _context.Notes.Remove(note);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}

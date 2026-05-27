using JodWai.Application.Interfaces;
using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace JodWai.Infrastructure.Persistence.Repositories;

internal class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context;
    public NoteRepository(AppDbContext context) => _context = context;

    public async Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default)
        => (await _context.Notes.AddAsync(note, cancellationToken)).Entity;

    public async Task<IEnumerable<Note>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Notes.AsNoTracking()
        .ToListAsync(cancellationToken);

    public async Task<Note?> GetByIdAsync(NoteId id, CancellationToken cancellationToken = default)
        => await _context.Notes.FirstOrDefaultAsync(n => n.Id.Value == id.Value, cancellationToken);

    public async Task<Guid?> GetIdByTitleAsync(string title, CancellationToken cancellationToken = default)
        => await _context.Notes.Where(n => n.Title.Value == title)
        .Select(n => n.Id.Value)
        .FirstOrDefaultAsync(cancellationToken);

    public void Update(Note note)
        => _context.Notes.Update(note);
    public void Delete(Note note)
        => _context.Notes.Remove(note);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}

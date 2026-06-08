using JodWai.Application.Notes.Commands;
using JodWai.Application.Notes.Dtos;
using JodWai.Application.Notes.Dtos.Requests;
using JodWai.Application.Notes.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace JodWai.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var notes = await _sender.Send(new GetAllNotesQuery(), cancellationToken);

        return Ok(notes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var note = await _sender.Send(new GetNoteByIdQuery(id), cancellationToken);

        if (note is null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    [HttpPost]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateNoteRequest request,
        CancellationToken cancellationToken)
    {
        var note = await _sender.Send(
            new CreateNoteCommand(request),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = note.Id },
            note);
    }

    [HttpPut]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateNoteRequest request,
        CancellationToken cancellationToken)
    {
        var note = await _sender.Send(
            new UpdateNoteCommand(request),
            cancellationToken);

        if (note is null)
        {
            return NotFound();
        }

        return Ok(note);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
        new DeleteNoteCommand(id),
        cancellationToken);

        return NoContent();
    }
}

using JodWai.Application.Common.Results.Errors;
using JodWai.Application.Notes.Commands.CreateNote;
using JodWai.Application.Notes.Commands.DeleteNote;
using JodWai.Application.Notes.Commands.UpdateNote;
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
    public async Task<IActionResult> Get(
     [FromQuery] string? keyword,
     CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            var notes = await _sender.Send(
                new GetAllNotesQuery(),
                cancellationToken);

            return Ok(notes);
        }

        var searchResult = await _sender.Send(
            new SearchNotesQuery(keyword),
            cancellationToken);

        return Ok(searchResult);
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
        var result = await _sender.Send(
            new CreateNoteCommand(request),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
        nameof(GetById),
        new { id = result.Value.Id },
        result.Value);
    }

    [HttpPut]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateNoteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateNoteCommand(request),
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.Code switch
            {
                NoteErrors.NotFoundCode => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };

        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
        new DeleteNoteCommand(id),
        cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.Code switch
            {
                NoteErrors.NotFoundCode => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return NoContent();
    }
}

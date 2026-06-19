using MainService.Application.Common;
using MainService.Application.Features.ClassRooms;
using MainService.Application.Features.ClassRooms.Commands;
using MainService.Application.Features.ClassRooms.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Api.Features.ClassRooms;

[ApiController]
[Route("classrooms")]
public sealed class ClassRoomsController(
    CreateClassRoomCommandHandler createClassRoomCommandHandler,
    UpdateClassRoomCommandHandler updateClassRoomCommandHandler,
    DeleteClassRoomCommandHandler deleteClassRoomCommandHandler,
    GetClassRoomByIdQueryHandler getClassRoomByIdQueryHandler,
    GetClassRoomsQueryHandler getClassRoomsQueryHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ClassRoomDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClassRoomDto>> Create(
        CreateClassRoomRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var classRoom = await createClassRoomCommandHandler.Handle(
                new CreateClassRoomCommand(request.Title, request.AdminId),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = classRoom.Id }, classRoom);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { exception.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<ClassRoomDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClassRoomDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var classRooms = await getClassRoomsQueryHandler.Handle(new GetClassRoomsQuery(), cancellationToken);

        return Ok(classRooms);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ClassRoomDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassRoomDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var classRoom = await getClassRoomByIdQueryHandler.Handle(new GetClassRoomByIdQuery(id), cancellationToken);

        if (classRoom is null)
        {
            return NotFound();
        }

        return Ok(classRoom);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ClassRoomDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassRoomDto>> Update(
        Guid id,
        UpdateClassRoomRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var classRoom = await updateClassRoomCommandHandler.Handle(
                new UpdateClassRoomCommand(id, request.Title, request.AdminId),
                cancellationToken);

            return Ok(classRoom);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { exception.Message });
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
        catch (ForbiddenAccessException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status403Forbidden);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] Guid adminId,
        CancellationToken cancellationToken)
    {
        try
        {
            await deleteClassRoomCommandHandler.Handle(new DeleteClassRoomCommand(id, adminId), cancellationToken);

            return NoContent();
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
        catch (ForbiddenAccessException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status403Forbidden);
        }
    }
}

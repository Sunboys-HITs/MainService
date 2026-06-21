using MainService.Application.Common;
using MainService.Application.Features.Tasks;
using MainService.Application.Features.Tasks.Commands;
using MainService.Application.Features.Tasks.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Api.Features.Tasks;

[ApiController]
public sealed class TasksController(
    CreateTaskCommandHandler createTaskCommandHandler,
    UpdateTaskCommandHandler updateTaskCommandHandler,
    DeleteTaskCommandHandler deleteTaskCommandHandler,
    GetTaskByIdQueryHandler getTaskByIdQueryHandler,
    GetClassRoomTasksQueryHandler getClassRoomTasksQueryHandler) : ControllerBase
{
    [HttpPost("classrooms/{classRoomId:guid}/tasks")]
    [ProducesResponseType<TaskDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Create(
        Guid classRoomId,
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await createTaskCommandHandler.Handle(
                new CreateTaskCommand(
                    classRoomId,
                    request.Title,
                    request.Description,
                    request.InputSample,
                    request.OutputSample,
                    request.Tests),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { exception.Message });
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
    }

    [HttpGet("tasks/{id:guid}")]
    [ProducesResponseType<TaskDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var task = await getTaskByIdQueryHandler.Handle(new GetTaskByIdQuery(id), cancellationToken);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpGet("classrooms/{classRoomId:guid}/tasks")]
    [ProducesResponseType<IReadOnlyCollection<TaskDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<TaskDto>>> GetByClassRoomId(
        Guid classRoomId,
        CancellationToken cancellationToken)
    {
        try
        {
            var tasks = await getClassRoomTasksQueryHandler.Handle(
                new GetClassRoomTasksQuery(classRoomId),
                cancellationToken);

            return Ok(tasks);
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
    }

    [HttpPut("tasks/{id:guid}")]
    [ProducesResponseType<TaskDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Update(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await updateTaskCommandHandler.Handle(
                new UpdateTaskCommand(
                    id,
                    request.Title,
                    request.Description,
                    request.InputSample,
                    request.OutputSample,
                    request.Tests),
                cancellationToken);

            return Ok(task);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { exception.Message });
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
    }

    [HttpDelete("tasks/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await deleteTaskCommandHandler.Handle(new DeleteTaskCommand(id), cancellationToken);

            return NoContent();
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
    }
}

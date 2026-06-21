using MainService.Application.Common;
using MainService.Application.Features.Solutions;
using MainService.Application.Features.Solutions.Commands;
using MainService.Application.Features.Solutions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MainService.Api.Features.Solutions;

[ApiController]
public sealed class SolutionsController(
    CreateSolutionCommandHandler createSolutionCommandHandler,
    UpdateSolutionStatusCommandHandler updateSolutionStatusCommandHandler,
    GetSolutionByIdQueryHandler getSolutionByIdQueryHandler,
    GetTaskSolutionsQueryHandler getTaskSolutionsQueryHandler,
    GetUserSolutionsQueryHandler getUserSolutionsQueryHandler) : ControllerBase
{
    [HttpPost("tasks/{taskId:guid}/solutions")]
    [ProducesResponseType<SolutionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolutionDto>> Create(
        Guid taskId,
        CreateSolutionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var solution = await createSolutionCommandHandler.Handle(
                new CreateSolutionCommand(
                    taskId,
                    request.UserId,
                    request.Language,
                    request.Code),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = solution.Id }, solution);
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

    [HttpGet("solutions/{id:guid}")]
    [ProducesResponseType<SolutionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolutionDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var solution = await getSolutionByIdQueryHandler.Handle(new GetSolutionByIdQuery(id), cancellationToken);

        if (solution is null)
        {
            return NotFound();
        }

        return Ok(solution);
    }

    [HttpGet("tasks/{taskId:guid}/solutions")]
    [ProducesResponseType<IReadOnlyCollection<SolutionDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SolutionDto>>> GetByTaskId(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        try
        {
            var solutions = await getTaskSolutionsQueryHandler.Handle(
                new GetTaskSolutionsQuery(taskId),
                cancellationToken);

            return Ok(solutions);
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(new { exception.Message });
        }
    }

    [HttpGet("users/{userId:guid}/solutions")]
    [ProducesResponseType<IReadOnlyCollection<SolutionDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SolutionDto>>> GetByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var solutions = await getUserSolutionsQueryHandler.Handle(
            new GetUserSolutionsQuery(userId),
            cancellationToken);

        return Ok(solutions);
    }

    [HttpPatch("solutions/{id:guid}/status")]
    [ProducesResponseType<SolutionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolutionDto>> UpdateStatus(
        Guid id,
        UpdateSolutionStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var solution = await updateSolutionStatusCommandHandler.Handle(
                new UpdateSolutionStatusCommand(id, request.Status),
                cancellationToken);

            return Ok(solution);
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
}

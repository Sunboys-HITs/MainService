using MainService.Application.Common;
using MainService.Db.Domain;
using MainService.Db.Repositories;
using MainService.Metrics;

namespace MainService.Application.Features.Solutions.Commands;

public sealed record CreateSolutionCommand(
    Guid TaskId,
    Guid UserId,
    ProgrammingLanguage Language,
    string Code);

public sealed class CreateSolutionCommandHandler(
    ITaskRepository taskRepository,
    ISolutionRepository solutionRepository,
    ISolutionExecutionPublisher solutionExecutionPublisher)
{
    public async Task<SolutionDto> Handle(CreateSolutionCommand command, CancellationToken cancellationToken)
    {
        var code = command.Code.Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Solution code is required.", nameof(command));
        }

        if (!Enum.IsDefined(command.Language))
        {
            throw new ArgumentException("Solution language is not supported.", nameof(command));
        }

        var task = await taskRepository.GetByIdAsync(command.TaskId, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException("Task was not found.");
        }

        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            Status = SolutionStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Language = command.Language,
            Code = code,
        };

        var createdSolution = await solutionRepository.CreateAsync(solution, cancellationToken);
        MainServiceMetrics.SolutionsCreatedTotal
            .WithLabels(createdSolution.Language.ToString())
            .Inc();

        await solutionExecutionPublisher.PublishAsync(
            createdSolution.TaskId,
            createdSolution.Id,
            createdSolution.Language,
            createdSolution.Code,
            cancellationToken);

        return createdSolution.ToDto();
    }
}

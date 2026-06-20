using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Solutions.Queries;

public sealed record GetTaskSolutionsQuery(Guid TaskId);

public sealed class GetTaskSolutionsQueryHandler(
    ITaskRepository taskRepository,
    ISolutionRepository solutionRepository)
{
    public async Task<IReadOnlyCollection<SolutionDto>> Handle(
        GetTaskSolutionsQuery query,
        CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(query.TaskId, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException("Task was not found.");
        }

        var solutions = await solutionRepository.GetByTaskIdAsync(query.TaskId, cancellationToken);

        return solutions.Select(solution => solution.ToDto()).ToArray();
    }
}

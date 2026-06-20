using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Queries;

public sealed record GetTaskByIdQuery(Guid Id);

public sealed class GetTaskByIdQueryHandler(ITaskRepository taskRepository)
{
    public async Task<TaskDto?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(query.Id, cancellationToken);

        return task?.ToDto();
    }
}

using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Commands;

public sealed record DeleteTaskCommand(
    Guid Id);

public sealed class DeleteTaskCommandHandler(ITaskRepository taskRepository)
{
    public async Task Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(command.Id, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException("Task was not found.");
        }

        await taskRepository.DeleteAsync(command.Id, cancellationToken);
    }
}

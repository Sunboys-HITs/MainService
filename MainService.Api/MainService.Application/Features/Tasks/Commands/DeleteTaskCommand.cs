using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Commands;

public sealed record DeleteTaskCommand(
    Guid Id,
    Guid AdminId);

public sealed class DeleteTaskCommandHandler(
    IClassRoomRepository classRoomRepository,
    ITaskRepository taskRepository)
{
    public async Task Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(command.Id, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException("Task was not found.");
        }

        var isAdmin = await classRoomRepository.IsAdminAsync(
            task.ClassRoomId,
            command.AdminId,
            cancellationToken);

        if (!isAdmin)
        {
            throw new ForbiddenAccessException("Only classroom admins can delete tasks.");
        }

        await taskRepository.DeleteAsync(command.Id, cancellationToken);
    }
}

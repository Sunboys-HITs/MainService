using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Queries;

public sealed record GetClassRoomTasksQuery(Guid ClassRoomId);

public sealed class GetClassRoomTasksQueryHandler(
    IClassRoomRepository classRoomRepository,
    ITaskRepository taskRepository)
{
    public async Task<IReadOnlyCollection<TaskDto>> Handle(
        GetClassRoomTasksQuery query,
        CancellationToken cancellationToken)
    {
        var classRoom = await classRoomRepository.GetByIdAsync(query.ClassRoomId, cancellationToken);

        if (classRoom is null)
        {
            throw new EntityNotFoundException("Classroom was not found.");
        }

        var tasks = await taskRepository.GetByClassRoomIdAsync(query.ClassRoomId, cancellationToken);

        return tasks.Select(task => task.ToDto()).ToArray();
    }
}

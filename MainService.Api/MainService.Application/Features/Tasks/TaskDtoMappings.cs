namespace MainService.Application.Features.Tasks;

public static class TaskDtoMappings
{
    public static TaskDto ToDto(this Db.Domain.Task task)
    {
        return new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.InputSample,
            task.OutputSample,
            task.Tests,
            task.ClassRoomId,
            task.Solutions.Count);
    }
}

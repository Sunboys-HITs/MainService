using MainService.Db.DbModels;

namespace MainService.Db.Mappings;

public static class TaskMappings
{
    public static Domain.Task ToDomain(this TaskDbModel dbModel)
    {
        return new Domain.Task
        {
            Id = dbModel.Id,
            Title = dbModel.Title,
            Description = dbModel.Description,
            InputSample = dbModel.InputSample,
            OutputSample = dbModel.OutputSample,
            Tests = dbModel.Tests,
            ClassRoomId = dbModel.ClassRoomId,
            Solutions = dbModel.Solutions.Select(solution => solution.ToDomain()).ToArray(),
        };
    }

    public static TaskDbModel ToDbModel(this Domain.Task domain)
    {
        return new TaskDbModel
        {
            Id = domain.Id,
            Title = domain.Title,
            Description = domain.Description,
            InputSample = domain.InputSample,
            OutputSample = domain.OutputSample,
            Tests = domain.Tests,
            ClassRoomId = domain.ClassRoomId,
            Solutions = domain.Solutions.Select(solution => solution.ToDbModel()).ToArray(),
        };
    }
}

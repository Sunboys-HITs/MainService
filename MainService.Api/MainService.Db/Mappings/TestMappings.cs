using MainService.Db.DbModels;

namespace MainService.Db.Mappings;

public static class TestMappings
{
    public static Domain.Test ToDomain(this TestDbModel dbModel)
    {
        return new Domain.Test
        {
            Id = dbModel.Id,
            TaskId = dbModel.TaskId,
            InputData = dbModel.InputData,
            ExpectedOutput = dbModel.ExpectedOutput,
        };
    }

    public static TestDbModel ToDbModel(this Domain.Test domain)
    {
        return new TestDbModel
        {
            Id = domain.Id,
            TaskId = domain.TaskId,
            InputData = domain.InputData,
            ExpectedOutput = domain.ExpectedOutput,
        };
    }
}

using MainService.Db.DbModels;
using MainService.Db.Domain;

namespace MainService.Db.Mappings;

public static class SolutionMappings
{
    public static Solution ToDomain(this SolutionDbModel dbModel)
    {
        return new Solution
        {
            Id = dbModel.Id,
            Status = (SolutionStatus)dbModel.Status,
            CreatedAt = dbModel.CreatedAt,
            TaskId = dbModel.TaskId,
            UserId = dbModel.UserId,
            Language = (ProgrammingLanguage)dbModel.Language,
            Code = dbModel.Code,
        };
    }

    public static SolutionDbModel ToDbModel(this Solution domain)
    {
        return new SolutionDbModel
        {
            Id = domain.Id,
            Status = (SolutionStatusDbModel)domain.Status,
            CreatedAt = domain.CreatedAt,
            TaskId = domain.TaskId,
            UserId = domain.UserId,
            Language = (ProgrammingLanguageDbModel)domain.Language,
            Code = domain.Code,
        };
    }
}

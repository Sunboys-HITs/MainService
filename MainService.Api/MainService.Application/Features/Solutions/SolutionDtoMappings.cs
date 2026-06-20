namespace MainService.Application.Features.Solutions;

public static class SolutionDtoMappings
{
    public static SolutionDto ToDto(this Db.Domain.Solution solution)
    {
        return new SolutionDto(
            solution.Id,
            solution.Status,
            solution.CreatedAt,
            solution.TaskId,
            solution.UserId,
            solution.Language,
            solution.Code);
    }
}

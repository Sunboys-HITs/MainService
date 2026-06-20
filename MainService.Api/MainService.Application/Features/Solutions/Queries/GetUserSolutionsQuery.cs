using MainService.Db.Repositories;

namespace MainService.Application.Features.Solutions.Queries;

public sealed record GetUserSolutionsQuery(Guid UserId);

public sealed class GetUserSolutionsQueryHandler(ISolutionRepository solutionRepository)
{
    public async Task<IReadOnlyCollection<SolutionDto>> Handle(
        GetUserSolutionsQuery query,
        CancellationToken cancellationToken)
    {
        var solutions = await solutionRepository.GetByUserIdAsync(query.UserId, cancellationToken);

        return solutions.Select(solution => solution.ToDto()).ToArray();
    }
}

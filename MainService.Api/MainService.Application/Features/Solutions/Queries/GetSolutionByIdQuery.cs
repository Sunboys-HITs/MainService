using MainService.Db.Repositories;

namespace MainService.Application.Features.Solutions.Queries;

public sealed record GetSolutionByIdQuery(Guid Id);

public sealed class GetSolutionByIdQueryHandler(ISolutionRepository solutionRepository)
{
    public async Task<SolutionDto?> Handle(GetSolutionByIdQuery query, CancellationToken cancellationToken)
    {
        var solution = await solutionRepository.GetByIdAsync(query.Id, cancellationToken);

        return solution?.ToDto();
    }
}

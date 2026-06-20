using MainService.Db.Domain;

namespace MainService.Db.Repositories;

public interface ISolutionRepository
{
    Task<Solution> CreateAsync(Solution solution, CancellationToken cancellationToken);
    Task<Solution?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Solution>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Solution>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Solution?> UpdateStatusAsync(
        Guid id,
        SolutionStatus status,
        CancellationToken cancellationToken);
}

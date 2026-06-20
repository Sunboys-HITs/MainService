using MainService.Db.Data;
using MainService.Db.DbModels;
using MainService.Db.Domain;
using MainService.Db.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MainService.Db.Repositories;

public sealed class SolutionRepository(MainServiceDbContext dbContext) : ISolutionRepository
{
    public async Task<Solution> CreateAsync(Solution solution, CancellationToken cancellationToken)
    {
        var dbModel = solution.ToDbModel();

        dbContext.Solutions.Add(dbModel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(dbModel.Id, cancellationToken))!;
    }

    public async Task<Solution?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.Solutions
            .AsNoTracking()
            .SingleOrDefaultAsync(solution => solution.Id == id, cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Solution>> GetByTaskIdAsync(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var dbModels = await dbContext.Solutions
            .AsNoTracking()
            .Where(solution => solution.TaskId == taskId)
            .OrderByDescending(solution => solution.CreatedAt)
            .ToArrayAsync(cancellationToken);

        return dbModels.Select(solution => solution.ToDomain()).ToArray();
    }

    public async Task<IReadOnlyCollection<Solution>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var dbModels = await dbContext.Solutions
            .AsNoTracking()
            .Where(solution => solution.UserId == userId)
            .OrderByDescending(solution => solution.CreatedAt)
            .ToArrayAsync(cancellationToken);

        return dbModels.Select(solution => solution.ToDomain()).ToArray();
    }

    public async Task<Solution?> UpdateStatusAsync(
        Guid id,
        SolutionStatus status,
        CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.Solutions
            .SingleOrDefaultAsync(solution => solution.Id == id, cancellationToken);

        if (dbModel is null)
        {
            return null;
        }

        dbModel.Status = (SolutionStatusDbModel)status;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }
}

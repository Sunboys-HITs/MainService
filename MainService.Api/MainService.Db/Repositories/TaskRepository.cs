using MainService.Db.Data;
using MainService.Db.Mappings;
using Microsoft.EntityFrameworkCore;
using TaskDomain = MainService.Db.Domain.Task;

namespace MainService.Db.Repositories;

public sealed class TaskRepository(MainServiceDbContext dbContext) : ITaskRepository
{
    public async Task<TaskDomain> CreateAsync(TaskDomain task, CancellationToken cancellationToken)
    {
        var dbModel = task.ToDbModel();

        dbContext.Tasks.Add(dbModel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(dbModel.Id, cancellationToken))!;
    }

    public async Task<TaskDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.Solutions)
            .SingleOrDefaultAsync(task => task.Id == id, cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<IReadOnlyCollection<TaskDomain>> GetByClassRoomIdAsync(
        Guid classRoomId,
        CancellationToken cancellationToken)
    {
        var dbModels = await dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.Solutions)
            .Where(task => task.ClassRoomId == classRoomId)
            .OrderBy(task => task.Title)
            .ToArrayAsync(cancellationToken);

        return dbModels.Select(task => task.ToDomain()).ToArray();
    }
}

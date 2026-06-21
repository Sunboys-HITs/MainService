using MainService.Db.Data;
using MainService.Db.Mappings;
using Microsoft.EntityFrameworkCore;
using TaskDomain = MainService.Db.Domain.Task;
using TestDomain = MainService.Db.Domain.Test;

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
            .Include(task => task.TestCases)
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
            .Include(task => task.TestCases)
            .Where(task => task.ClassRoomId == classRoomId)
            .OrderBy(task => task.Title)
            .ToArrayAsync(cancellationToken);

        return dbModels.Select(task => task.ToDomain()).ToArray();
    }

    public async Task<TaskDomain?> UpdateAsync(
        Guid id,
        string title,
        string description,
        string? inputSample,
        string? outputSample,
        string tests,
        IReadOnlyCollection<TestDomain> testCases,
        CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.Tasks
            .Include(task => task.TestCases)
            .SingleOrDefaultAsync(task => task.Id == id, cancellationToken);

        if (dbModel is null)
        {
            return null;
        }

        dbModel.Title = title;
        dbModel.Description = description;
        dbModel.InputSample = inputSample;
        dbModel.OutputSample = outputSample;
        dbModel.Tests = tests;
        dbContext.Tests.RemoveRange(dbModel.TestCases);
        dbModel.TestCases = testCases.Select(testCase => testCase.ToDbModel()).ToArray();

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.Tasks
            .SingleOrDefaultAsync(task => task.Id == id, cancellationToken);

        if (dbModel is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(dbModel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

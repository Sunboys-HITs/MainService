using TaskDomain = MainService.Db.Domain.Task;
using TestDomain = MainService.Db.Domain.Test;

namespace MainService.Db.Repositories;

public interface ITaskRepository
{
    Task<TaskDomain> CreateAsync(TaskDomain task, CancellationToken cancellationToken);
    Task<TaskDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TaskDomain>> GetByClassRoomIdAsync(Guid classRoomId, CancellationToken cancellationToken);
    Task<TaskDomain?> UpdateAsync(
        Guid id,
        string title,
        string description,
        string? inputSample,
        string? outputSample,
        string tests,
        IReadOnlyCollection<TestDomain> testCases,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

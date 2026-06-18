using MainService.Db.Domain;

namespace MainService.Db.Repositories;

public interface IClassRoomRepository
{
    Task<ClassRoom> CreateAsync(ClassRoom classRoom, CancellationToken cancellationToken);
    Task<ClassRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ClassRoom>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClassRoom?> UpdateTitleAsync(Guid id, string title, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsAdminAsync(Guid classRoomId, Guid userId, CancellationToken cancellationToken);
}

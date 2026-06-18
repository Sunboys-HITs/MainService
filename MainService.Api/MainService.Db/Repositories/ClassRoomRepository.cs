using MainService.Db.Data;
using MainService.Db.DbModels;
using MainService.Db.Domain;
using MainService.Db.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MainService.Db.Repositories;

public sealed class ClassRoomRepository(MainServiceDbContext dbContext) : IClassRoomRepository
{
    public async Task<ClassRoom> CreateAsync(ClassRoom classRoom, CancellationToken cancellationToken)
    {
        var dbModel = classRoom.ToDbModel();

        dbContext.ClassRooms.Add(dbModel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(dbModel.Id, cancellationToken))!;
    }

    public async Task<ClassRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.ClassRooms
            .AsNoTracking()
            .Include(classRoom => classRoom.Members)
            .Include(classRoom => classRoom.Tasks)
            .SingleOrDefaultAsync(classRoom => classRoom.Id == id, cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<IReadOnlyCollection<ClassRoom>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbModels = await dbContext.ClassRooms
            .AsNoTracking()
            .Include(classRoom => classRoom.Members)
            .Include(classRoom => classRoom.Tasks)
            .OrderBy(classRoom => classRoom.Title)
            .ToArrayAsync(cancellationToken);

        return dbModels.Select(classRoom => classRoom.ToDomain()).ToArray();
    }

    public async Task<ClassRoom?> UpdateTitleAsync(Guid id, string title, CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.ClassRooms
            .SingleOrDefaultAsync(classRoom => classRoom.Id == id, cancellationToken);

        if (dbModel is null)
        {
            return null;
        }

        dbModel.Title = title;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbModel = await dbContext.ClassRooms
            .SingleOrDefaultAsync(classRoom => classRoom.Id == id, cancellationToken);

        if (dbModel is null)
        {
            return false;
        }

        dbContext.ClassRooms.Remove(dbModel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> IsAdminAsync(Guid classRoomId, Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.ClassRoomMembers.AnyAsync(
            member => member.ClassRoomId == classRoomId
                && member.UserId == userId
                && member.Role == ClassRoomMemberRoleDbModel.Admin,
            cancellationToken);
    }
}

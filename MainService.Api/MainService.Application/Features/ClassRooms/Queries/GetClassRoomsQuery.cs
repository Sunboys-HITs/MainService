using MainService.Db.Repositories;

namespace MainService.Application.Features.ClassRooms.Queries;

public sealed record GetClassRoomsQuery;

public sealed class GetClassRoomsQueryHandler(IClassRoomRepository classRoomRepository)
{
    public async Task<IReadOnlyCollection<ClassRoomDto>> Handle(GetClassRoomsQuery query, CancellationToken cancellationToken)
    {
        var classRooms = await classRoomRepository.GetAllAsync(cancellationToken);

        return classRooms.Select(classRoom => classRoom.ToDto()).ToArray();
    }
}

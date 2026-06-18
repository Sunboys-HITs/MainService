using MainService.Db.Repositories;

namespace MainService.Application.Features.ClassRooms.Queries;

public sealed record GetClassRoomByIdQuery(Guid Id);

public sealed class GetClassRoomByIdQueryHandler(IClassRoomRepository classRoomRepository)
{
    public async Task<ClassRoomDto?> Handle(GetClassRoomByIdQuery query, CancellationToken cancellationToken)
    {
        var classRoom = await classRoomRepository.GetByIdAsync(query.Id, cancellationToken);

        return classRoom?.ToDto();
    }
}

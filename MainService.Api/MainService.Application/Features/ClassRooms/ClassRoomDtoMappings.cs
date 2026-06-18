using MainService.Db.Domain;

namespace MainService.Application.Features.ClassRooms;

public static class ClassRoomDtoMappings
{
    public static ClassRoomDto ToDto(this ClassRoom classRoom)
    {
        return new ClassRoomDto(
            classRoom.Id,
            classRoom.Title,
            classRoom.Members
                .Select(member => new ClassRoomMemberDto(member.UserId, member.Role.ToString()))
                .ToArray(),
            classRoom.Tasks.Count);
    }
}

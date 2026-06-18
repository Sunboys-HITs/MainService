namespace MainService.Application.Features.ClassRooms;

public sealed record ClassRoomDto(
    Guid Id,
    string Title,
    IReadOnlyCollection<ClassRoomMemberDto> Members,
    int TasksCount);

public sealed record ClassRoomMemberDto(
    Guid UserId,
    string Role);

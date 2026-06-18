namespace MainService.Api.Features.ClassRooms;

public sealed record CreateClassRoomRequest(
    string Title,
    Guid AdminId);

namespace MainService.Api.Features.ClassRooms;

public sealed record UpdateClassRoomRequest(
    string Title,
    Guid AdminId);

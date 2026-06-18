using MainService.Db.Domain;
using MainService.Db.Repositories;

namespace MainService.Application.Features.ClassRooms.Commands;

public sealed record CreateClassRoomCommand(
    string Title,
    Guid AdminId);

public sealed class CreateClassRoomCommandHandler(IClassRoomRepository classRoomRepository)
{
    public async Task<ClassRoomDto> Handle(CreateClassRoomCommand command, CancellationToken cancellationToken)
    {
        var title = command.Title.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Classroom title is required.", nameof(command));
        }

        var classRoomId = Guid.NewGuid();
        var classRoom = new ClassRoom
        {
            Id = classRoomId,
            Title = title,
            Members =
            [
                new ClassRoomMember
                {
                    UserId = command.AdminId,
                    ClassRoomId = classRoomId,
                    Role = ClassRoomMemberRole.Admin,
                },
            ],
        };

        var createdClassRoom = await classRoomRepository.CreateAsync(classRoom, cancellationToken);

        return createdClassRoom.ToDto();
    }
}

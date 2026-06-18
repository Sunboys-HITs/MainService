using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.ClassRooms.Commands;

public sealed record DeleteClassRoomCommand(
    Guid Id,
    Guid AdminId);

public sealed class DeleteClassRoomCommandHandler(IClassRoomRepository classRoomRepository)
{
    public async Task Handle(DeleteClassRoomCommand command, CancellationToken cancellationToken)
    {
        var classRoom = await classRoomRepository.GetByIdAsync(command.Id, cancellationToken);

        if (classRoom is null)
        {
            throw new EntityNotFoundException("Classroom was not found.");
        }

        var isAdmin = await classRoomRepository.IsAdminAsync(command.Id, command.AdminId, cancellationToken);

        if (!isAdmin)
        {
            throw new ForbiddenAccessException("Only classroom admins can delete classroom.");
        }

        await classRoomRepository.DeleteAsync(command.Id, cancellationToken);
    }
}

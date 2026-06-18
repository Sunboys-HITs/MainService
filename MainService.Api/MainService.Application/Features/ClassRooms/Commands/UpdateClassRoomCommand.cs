using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.ClassRooms.Commands;

public sealed record UpdateClassRoomCommand(
    Guid Id,
    string Title,
    Guid AdminId);

public sealed class UpdateClassRoomCommandHandler(IClassRoomRepository classRoomRepository)
{
    public async Task<ClassRoomDto> Handle(UpdateClassRoomCommand command, CancellationToken cancellationToken)
    {
        var title = command.Title.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Classroom title is required.", nameof(command));
        }

        var classRoom = await classRoomRepository.GetByIdAsync(command.Id, cancellationToken);

        if (classRoom is null)
        {
            throw new EntityNotFoundException("Classroom was not found.");
        }

        var isAdmin = await classRoomRepository.IsAdminAsync(command.Id, command.AdminId, cancellationToken);

        if (!isAdmin)
        {
            throw new ForbiddenAccessException("Only classroom admins can update classroom.");
        }

        var updatedClassRoom = await classRoomRepository.UpdateTitleAsync(command.Id, title, cancellationToken);

        return updatedClassRoom!.ToDto();
    }
}

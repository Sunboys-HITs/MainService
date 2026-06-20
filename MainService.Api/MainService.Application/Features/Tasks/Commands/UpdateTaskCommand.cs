using System.Text.Json;
using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Commands;

public sealed record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    string? InputSample,
    string? OutputSample,
    string Tests,
    Guid AdminId);

public sealed class UpdateTaskCommandHandler(
    IClassRoomRepository classRoomRepository,
    ITaskRepository taskRepository)
{
    public async Task<TaskDto> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var title = command.Title.Trim();
        var description = command.Description.Trim();
        var tests = command.Tests.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Task description is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(tests))
        {
            throw new ArgumentException("Task tests are required.", nameof(command));
        }

        ValidateTestsJson(tests);

        var task = await taskRepository.GetByIdAsync(command.Id, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException("Task was not found.");
        }

        var isAdmin = await classRoomRepository.IsAdminAsync(
            task.ClassRoomId,
            command.AdminId,
            cancellationToken);

        if (!isAdmin)
        {
            throw new ForbiddenAccessException("Only classroom admins can update tasks.");
        }

        var updatedTask = await taskRepository.UpdateAsync(
            command.Id,
            title,
            description,
            NormalizeOptionalText(command.InputSample),
            NormalizeOptionalText(command.OutputSample),
            tests,
            cancellationToken);

        return updatedTask!.ToDto();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static void ValidateTestsJson(string tests)
    {
        try
        {
            using var document = JsonDocument.Parse(tests);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Task tests must be valid JSON.", nameof(tests), exception);
        }
    }
}

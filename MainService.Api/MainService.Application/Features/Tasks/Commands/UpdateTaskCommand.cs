using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Commands;

public sealed record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    string? InputSample,
    string? OutputSample,
    string Tests);

public sealed class UpdateTaskCommandHandler(ITaskRepository taskRepository)
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

        var task = await taskRepository.GetByIdAsync(command.Id, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException("Task was not found.");
        }

        var testCases = TaskTestsParser.Parse(command.Id, tests);

        var updatedTask = await taskRepository.UpdateAsync(
            command.Id,
            title,
            description,
            NormalizeOptionalText(command.InputSample),
            NormalizeOptionalText(command.OutputSample),
            tests,
            testCases,
            cancellationToken);

        return updatedTask!.ToDto();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

}

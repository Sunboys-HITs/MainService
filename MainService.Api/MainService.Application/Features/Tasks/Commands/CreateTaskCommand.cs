using MainService.Application.Common;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Tasks.Commands;

public sealed record CreateTaskCommand(
    Guid ClassRoomId,
    string Title,
    string Description,
    string? InputSample,
    string? OutputSample,
    string Tests);

public sealed class CreateTaskCommandHandler(
    IClassRoomRepository classRoomRepository,
    ITaskRepository taskRepository)
{
    public async Task<TaskDto> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
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

        var classRoom = await classRoomRepository.GetByIdAsync(command.ClassRoomId, cancellationToken);

        if (classRoom is null)
        {
            throw new EntityNotFoundException("Classroom was not found.");
        }

        var taskId = Guid.NewGuid();
        var testCases = TaskTestsParser.Parse(taskId, tests);

        var task = new Db.Domain.Task
        {
            Id = taskId,
            Title = title,
            Description = description,
            InputSample = NormalizeOptionalText(command.InputSample),
            OutputSample = NormalizeOptionalText(command.OutputSample),
            Tests = tests,
            ClassRoomId = command.ClassRoomId,
            TestCases = testCases,
        };

        var createdTask = await taskRepository.CreateAsync(task, cancellationToken);

        return createdTask.ToDto();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

}

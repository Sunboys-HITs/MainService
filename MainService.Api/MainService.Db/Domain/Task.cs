namespace MainService.Db.Domain;

public class Task
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? InputSample { get; init; }
    public string? OutputSample { get; init; }
    public string Tests { get; init; } = string.Empty;
    public Guid ClassRoomId { get; init; }

    public IReadOnlyCollection<Solution> Solutions { get; init; } = [];
    public IReadOnlyCollection<Test> TestCases { get; init; } = [];
}

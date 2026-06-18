namespace MainService.Db.Domain;

public class ClassRoom
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;

    public IReadOnlyCollection<ClassRoomMember> Members { get; init; } = [];
    public IReadOnlyCollection<Task> Tasks { get; init; } = [];
}

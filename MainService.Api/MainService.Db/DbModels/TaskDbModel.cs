namespace MainService.Db.DbModels;

public class TaskDbModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? InputSample { get; set; }
    public string? OutputSample { get; set; }
    public string Tests { get; set; } = string.Empty;
    public Guid ClassRoomId { get; set; }

    public ClassRoomDbModel ClassRoom { get; set; } = null!;
    public ICollection<SolutionDbModel> Solutions { get; set; } = [];
}

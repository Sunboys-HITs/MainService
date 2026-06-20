namespace MainService.Db.DbModels;

public class TestDbModel
{
    public long Id { get; set; }
    public Guid TaskId { get; set; }
    public string InputData { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;

    public TaskDbModel Task { get; set; } = null!;
}

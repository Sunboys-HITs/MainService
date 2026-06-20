namespace MainService.Db.Domain;

public class Test
{
    public long Id { get; init; }
    public Guid TaskId { get; init; }
    public string InputData { get; init; } = string.Empty;
    public string ExpectedOutput { get; init; } = string.Empty;
}

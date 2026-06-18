namespace MainService.Db.DbModels;

public class SolutionDbModel
{
    public Guid Id { get; set; }
    public SolutionStatusDbModel Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public ProgrammingLanguageDbModel Language { get; set; }
    public string Code { get; set; } = string.Empty;

    public TaskDbModel Task { get; set; } = null!;
}

public enum SolutionStatusDbModel
{
    Pending,
    Accepted,
    Rejected,
}

public enum ProgrammingLanguageDbModel
{
    Cpp,
    CSharp,
    Python,
    Java,
    C,
    Go,
}

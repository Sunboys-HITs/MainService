namespace MainService.Db.Domain;

public class Solution
{
    public Guid Id { get; init; }
    public SolutionStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public ProgrammingLanguage Language { get; init; }
    public string Code { get; init; } = string.Empty;
}

public enum SolutionStatus
{
    Pending,
    Accepted,
    Rejected,
}

public enum ProgrammingLanguage
{
    Cpp,
    CSharp,
    Python,
    Java,
    C,
    Go,
}

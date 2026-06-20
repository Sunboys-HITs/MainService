using MainService.Db.Domain;

namespace MainService.Application.Features.Solutions;

public sealed record SolutionDto(
    Guid Id,
    SolutionStatus Status,
    DateTime CreatedAt,
    Guid TaskId,
    Guid UserId,
    ProgrammingLanguage Language,
    string Code);

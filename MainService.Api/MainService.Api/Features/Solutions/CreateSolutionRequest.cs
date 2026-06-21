using MainService.Db.Domain;

namespace MainService.Api.Features.Solutions;

public sealed record CreateSolutionRequest(
    Guid UserId,
    ProgrammingLanguage Language,
    string Code);

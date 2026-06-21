using MainService.Db.Domain;

namespace MainService.Api.Features.Solutions;

public sealed record UpdateSolutionStatusRequest(SolutionStatus Status);

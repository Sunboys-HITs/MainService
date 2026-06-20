namespace MainService.Application.Features.Tasks;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string Description,
    string? InputSample,
    string? OutputSample,
    string Tests,
    Guid ClassRoomId,
    int SolutionsCount);

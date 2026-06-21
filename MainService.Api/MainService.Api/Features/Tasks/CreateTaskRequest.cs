namespace MainService.Api.Features.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string Description,
    string? InputSample,
    string? OutputSample,
    string Tests);

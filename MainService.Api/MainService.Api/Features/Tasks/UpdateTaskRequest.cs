namespace MainService.Api.Features.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string Description,
    string? InputSample,
    string? OutputSample,
    string Tests);

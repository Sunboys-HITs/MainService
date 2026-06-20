using MainService.Application.Common;
using MainService.Db.Domain;
using MainService.Db.Repositories;

namespace MainService.Application.Features.Solutions.Commands;

public sealed record UpdateSolutionStatusCommand(
    Guid Id,
    SolutionStatus Status);

public sealed class UpdateSolutionStatusCommandHandler(ISolutionRepository solutionRepository)
{
    public async Task<SolutionDto> Handle(
        UpdateSolutionStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(command.Status))
        {
            throw new ArgumentException("Solution status is not supported.", nameof(command));
        }

        var solution = await solutionRepository.UpdateStatusAsync(
            command.Id,
            command.Status,
            cancellationToken);

        if (solution is null)
        {
            throw new EntityNotFoundException("Solution was not found.");
        }

        return solution.ToDto();
    }
}

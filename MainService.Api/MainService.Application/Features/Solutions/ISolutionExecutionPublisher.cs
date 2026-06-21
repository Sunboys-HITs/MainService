using MainService.Db.Domain;

namespace MainService.Application.Features.Solutions;

public interface ISolutionExecutionPublisher
{
    System.Threading.Tasks.Task PublishAsync(
        Guid taskId,
        Guid packageId,
        ProgrammingLanguage language,
        string code,
        CancellationToken cancellationToken);
}

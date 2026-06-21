using System;

namespace MainService.RabbitMq.Contracts;

public sealed record CodeExecutionRequest(
    string Code,
    string Language,
    string TaskId,
    Guid PackageId);

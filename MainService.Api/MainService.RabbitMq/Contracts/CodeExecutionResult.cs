using System;
using System.Collections.Generic;

namespace MainService.RabbitMq.Contracts;

public sealed class CodeExecutionResult
{
    public int PassedTests { get; set; }
    public int FailedTestsCount { get; set; }
    public List<FailedTest> FailedTests { get; set; } = [];
    public string CorrelationId { get; set; } = string.Empty;
    public Guid PackageId { get; set; }
}

public readonly struct FailedTest
{
    public Guid TestId { get; init; }
    public string Reason { get; init; }
}

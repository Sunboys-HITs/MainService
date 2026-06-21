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

public sealed class FailedTest
{
    public int TestNumber { get; set; }
    public string InputData { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public string ActualOutput { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? TestId { get; set; }
    public string? Reason { get; set; }
}

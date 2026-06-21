using System.Text.Json;
using MainService.Db.Domain;

namespace MainService.Application.Features.Tasks;

internal static class TaskTestsParser
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static IReadOnlyCollection<Test> Parse(Guid taskId, string tests)
    {
        TestCaseDto[]? testCases;

        try
        {
            testCases = JsonSerializer.Deserialize<TestCaseDto[]>(tests, JsonSerializerOptions);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Task tests must be valid JSON.", nameof(tests), exception);
        }

        if (testCases is null || testCases.Length == 0)
        {
            throw new ArgumentException("Task tests must contain at least one test case.", nameof(tests));
        }

        return testCases.Select(testCase =>
        {
            var inputData = testCase.InputData?.Trim();
            var expectedOutput = testCase.ExpectedOutput?.Trim();

            if (string.IsNullOrWhiteSpace(inputData))
            {
                throw new ArgumentException("Test inputData is required.", nameof(tests));
            }

            if (string.IsNullOrWhiteSpace(expectedOutput))
            {
                throw new ArgumentException("Test expectedOutput is required.", nameof(tests));
            }

            return new Test
            {
                TaskId = taskId,
                InputData = inputData,
                ExpectedOutput = expectedOutput,
            };
        }).ToArray();
    }

    private sealed record TestCaseDto(string? InputData, string? ExpectedOutput);
}

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MainService.Application.Common;
using MainService.Application.Features.Solutions.Commands;
using MainService.Metrics;
using MainService.RabbitMq.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SolutionStatus = MainService.Db.Domain.SolutionStatus;

namespace MainService.RabbitMq.Consumption;

internal sealed class CodeExecutionResultConsumer(
    RabbitMqConnectionFactory connectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<CodeExecutionResultConsumer> logger) : BackgroundService
{
    private readonly RabbitMqOptions options = options.Value;
    private IConnection? connection;
    private IModel? channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: options.ResultQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += HandleResultAsync;

        channel.BasicConsume(
            queue: options.ResultQueueName,
            autoAck: false,
            consumer: consumer);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public override void Dispose()
    {
        channel?.Dispose();
        connection?.Dispose();
        base.Dispose();
    }

    private async Task HandleResultAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        MainServiceMetrics.CodeExecutionResultsInProgress.Inc();

        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var result = DeserializeResult(json);

            if (result is null)
            {
                logger.LogWarning("Empty code execution result was received.");
                MainServiceMetrics.CodeExecutionResultFailuresTotal
                    .WithLabels("empty_result")
                    .Inc();
                channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
                return;
            }

            if (result.PackageId == Guid.Empty
                && Guid.TryParse(result.CorrelationId, out var correlationId))
            {
                result.PackageId = correlationId;
            }

            if (result.PackageId == Guid.Empty
                && Guid.TryParse(eventArgs.BasicProperties?.CorrelationId, out var messageCorrelationId))
            {
                result.PackageId = messageCorrelationId;
            }

            if (result.PackageId == Guid.Empty)
            {
                logger.LogWarning("Code execution result did not contain a package id. Payload: {Payload}", json);
                MainServiceMetrics.CodeExecutionResultFailuresTotal
                    .WithLabels("missing_package_id")
                    .Inc();
                channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
                return;
            }

            MainServiceMetrics.CodeExecutionTestsTotal
                .WithLabels("passed")
                .Inc(result.PassedTests);
            MainServiceMetrics.CodeExecutionTestsTotal
                .WithLabels("failed")
                .Inc(result.FailedTestsCount);

            using var scope = serviceScopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<UpdateSolutionStatusCommandHandler>();
            var status = result.FailedTestsCount == 0 && result.FailedTests.Count == 0
                ? SolutionStatus.Accepted
                : SolutionStatus.Rejected;

            await handler.Handle(
                new UpdateSolutionStatusCommand(result.PackageId, status),
                CancellationToken.None);

            MainServiceMetrics.CodeExecutionResultsConsumedTotal
                .WithLabels(status.ToString())
                .Inc();

            channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (EntityNotFoundException exception)
        {
            logger.LogWarning(exception, "Code execution result references an unknown solution.");
            MainServiceMetrics.CodeExecutionResultFailuresTotal
                .WithLabels("unknown_solution")
                .Inc();
            channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Invalid code execution result payload was received.");
            MainServiceMetrics.CodeExecutionResultFailuresTotal
                .WithLabels("bad_result_payload")
                .Inc();
            channel?.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to handle code execution result.");
            MainServiceMetrics.CodeExecutionResultFailuresTotal
                .WithLabels("handler_error")
                .Inc();
            channel?.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
        finally
        {
            MainServiceMetrics.CodeExecutionResultsInProgress.Dec();
        }
    }

    private static CodeExecutionResult? DeserializeResult(string json)
    {
        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        try
        {
            return JsonSerializer.Deserialize<CodeExecutionResult>(json, serializerOptions);
        }
        catch (JsonException)
        {
            var innerJson = JsonSerializer.Deserialize<string>(json, serializerOptions);

            if (string.IsNullOrWhiteSpace(innerJson))
            {
                throw;
            }

            return JsonSerializer.Deserialize<CodeExecutionResult>(innerJson, serializerOptions);
        }
    }
}

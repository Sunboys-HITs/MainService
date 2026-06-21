using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MainService.Application.Features.Solutions;
using MainService.Metrics;
using MainService.RabbitMq.Contracts;
using Microsoft.Extensions.Options;
using Prometheus;
using RabbitMQ.Client;
using ProgrammingLanguage = MainService.Db.Domain.ProgrammingLanguage;

namespace MainService.RabbitMq.Publishing;

internal sealed class RabbitMqCodeExecutionRequestPublisher(
    RabbitMqConnectionFactory connectionFactory,
    IOptions<RabbitMqOptions> options) : ISolutionExecutionPublisher
{
    private readonly RabbitMqOptions options = options.Value;

    public Task PublishAsync(
        Guid taskId,
        Guid packageId,
        ProgrammingLanguage language,
        string code,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var languageLabel = language.ToString();

        try
        {
            using var timer = MainServiceMetrics.CodeExecutionRequestPublishDurationSeconds
                .WithLabels(languageLabel)
                .NewTimer();

            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: options.RequestQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var request = new CodeExecutionRequest(
                code,
                languageLabel.ToLower(),
                taskId.ToString(),
                packageId);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.CorrelationId = packageId.ToString();
            properties.ContentType = "application/json";

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: options.RequestQueueName,
                basicProperties: properties,
                body: body);

            MainServiceMetrics.CodeExecutionRequestsPublishedTotal
                .WithLabels(languageLabel)
                .Inc();
        }
        catch
        {
            MainServiceMetrics.CodeExecutionRequestPublishFailuresTotal
                .WithLabels(languageLabel)
                .Inc();

            throw;
        }

        return Task.CompletedTask;
    }
}

using MainService.Application.Features.Solutions;
using MainService.RabbitMq.Consumption;
using MainService.RabbitMq.Publishing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainService.RabbitMq;

public static class RabbitMqServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqCodeExecution(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<RabbitMqConnectionFactory>();
        services.AddScoped<ISolutionExecutionPublisher, RabbitMqCodeExecutionRequestPublisher>();
        services.AddHostedService<CodeExecutionResultConsumer>();

        return services;
    }
}

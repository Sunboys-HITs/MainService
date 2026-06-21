using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MainService.RabbitMq;

internal sealed class RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
{
    private readonly RabbitMqOptions options = options.Value;

    public IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.UserName,
            Password = options.Password,
            DispatchConsumersAsync = true,
        };

        return factory.CreateConnection();
    }
}

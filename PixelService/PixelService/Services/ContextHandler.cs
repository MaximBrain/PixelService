using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PixelShared.Models;
using RabbitMQ.Client;

namespace PixelService.Services;

public record ContextHandler(IOptions<RabbitMQConfiguration> RabbitMqOptions)
{
    public void SendMessageToQueue(HttpRequestInfo httpRequestInfo)
    {
        var rabbitMq = RabbitMqOptions.Value ?? throw new InvalidOperationException("RabbitMQ configuration is missing");
        var message = JsonSerializer.Serialize(httpRequestInfo);
        var factory = new ConnectionFactory
        {
            Uri = new Uri($"{rabbitMq.Protocol}://{rabbitMq.UserName}:{rabbitMq.Password}@{rabbitMq.Host}:{rabbitMq.Port}"),
            ClientProvidedName = nameof(PixelService)
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
    
        channel.ExchangeDeclare(exchange: rabbitMq.ExchangeName, type: ExchangeType.Direct);
        channel.QueueDeclare(queue: rabbitMq.QueueName, false, false,false, null);
        channel.QueueBind(queue: rabbitMq.QueueName, exchange: rabbitMq.ExchangeName, routingKey: rabbitMq.RoutingKey);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: rabbitMq.ExchangeName,
            routingKey: rabbitMq.RoutingKey,
            basicProperties: null,
            body: body);
    
        channel.Close();
    }
}
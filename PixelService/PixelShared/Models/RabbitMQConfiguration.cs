namespace PixelShared.Models;

public record RabbitMQConfiguration
{
    public string? Protocol { get; init; }
    public string? Host { get; init; }
    public string? Port { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? QueueName { get; init; }
    public string? ExchangeName { get; init; }
    public string? RoutingKey { get; init; }
}

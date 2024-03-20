using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PixelShared.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StorageService.Services;

// Load configuration from appsettings.json and environment variables

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var rabbitMqConfig = new RabbitMQConfiguration();
configuration.GetSection("RabbitMQ").Bind(rabbitMqConfig);
var filePath = configuration.GetValue<string>("FilePath") ?? "/tmp/visits.log";

Console.WriteLine(JsonSerializer.Serialize(rabbitMqConfig));

// Ensure file path exists
Directory.CreateDirectory(Path.GetDirectoryName(filePath));

// Setup RabbitMQ connection
var factory = new ConnectionFactory
{
    Uri = new Uri($"{rabbitMqConfig.Protocol}://{rabbitMqConfig.UserName}:{rabbitMqConfig.Password}@" +
                  $"{rabbitMqConfig.Host}:{rabbitMqConfig.Port}"),
    ClientProvidedName = "StorageService"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
var logService = new LogService();
channel.ExchangeDeclare(exchange: rabbitMqConfig.ExchangeName, type: ExchangeType.Direct);
channel.QueueDeclare(queue: rabbitMqConfig.QueueName, false, false,false, null);
channel.QueueBind(queue: rabbitMqConfig.QueueName, exchange: rabbitMqConfig.ExchangeName, routingKey: rabbitMqConfig.RoutingKey);

// Setup event consumer
var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (_, ea) =>
{
    var body = ea.Body.ToArray();
    var message = JsonSerializer.Deserialize<HttpRequestInfo>(body);
    
    var logEntry = logService.GetLogEntry(message);

    await File.AppendAllTextAsync(filePath, logEntry);
};

channel.BasicConsume(queue: rabbitMqConfig.QueueName, autoAck: true, consumer: consumer);

// Keep the application running
await Task.Delay(-1);
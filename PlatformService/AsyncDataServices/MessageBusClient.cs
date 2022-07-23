using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
  private readonly IConfiguration _configuration;
  private readonly IConnection _connection;
  private readonly IModel _channel;

  public MessageBusClient(IConfiguration configuration)
  {
    _configuration = configuration;
    var factory = new ConnectionFactory()
      { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]) };

    try
    {
      _connection = factory.CreateConnection();
      _channel = _connection.CreateModel();
      _channel.ExchangeDeclare(exchange:"Trigger", type: ExchangeType.Fanout);

      _connection.ConnectionShutdown += OnRabbitMQConnShutdown;
    }
    catch(Exception ex)
    {
      Console.WriteLine($"Couldn't connect to Rabbit MQ: {ex.Message}");
    }
  }

  private void OnRabbitMQConnShutdown(object? sender, ShutdownEventArgs e)
  {
    Console.WriteLine($"RabbitMq connection closed: {e.Cause}");
  }

  public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
  {
    var serailizedMessage = JsonSerializer.Serialize(platformPublishDto);

    if (_connection.IsOpen && _channel.IsOpen)
    {
      Console.WriteLine("Sending message");
      SendMessage(serailizedMessage);
    }
    else
    {
      Console.WriteLine($"Connection closed, cannot send message: {_channel.CloseReason}");
    }
  }

  public void SendMessage(string message)
  {
    var body = Encoding.UTF8.GetBytes(message);
    _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
  }
}


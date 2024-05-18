using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Jacko.MessageBus;

public class MessageBus : IMessageBus
{

    //private readonly string connectionString = "<Your Azure Service Bus Connection String>";

    public string? ConnectionString { get; set; } = "";
    public string? HostName { get; set; } = ""; 
    public string? UserName { get; set; } = "";
    public string? Password { get; set; } = "";

    public async Task PublishMessage(object message, string topic_queue_name)
    {
        if (ConnectionString == "")
        {
            throw new Exception("Connection string is not provided for Azure service Bus");
        }
        await using var client = new ServiceBusClient(ConnectionString);

        ServiceBusSender sender = client.CreateSender(topic_queue_name);

        var jsonMessage = JsonConvert.SerializeObject(message);

        ServiceBusMessage servBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(servBusMessage);
        //await client.DisposeAsync();

    }
}


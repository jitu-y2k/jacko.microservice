using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Jacko.MessageBus;

public class AzureServiceMessageBus : IMessageBus
{

    private readonly string _connectionString;

    //public string? ConnectionString { get; set; } = "";
    //public string? HostName { get; set; } = ""; 
    //public string? UserName { get; set; } = "";
    //public string? Password { get; set; } = "";

    public AzureServiceMessageBus(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task PublishMessage(object message, string topic_queue_name)
    {
        if (_connectionString == "")
        {
            throw new Exception("Connection string is not provided for Azure service Bus");
        }
        await using var client = new ServiceBusClient(_connectionString);

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


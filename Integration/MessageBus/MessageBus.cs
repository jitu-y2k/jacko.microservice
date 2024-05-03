using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Jacko.MessageBus;

public class MessageBus : IMessageBus
{

    

    public async Task PublishMessage(object message, string topic_queue_name)
    {
        await using var client = new ServiceBusClient("");

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


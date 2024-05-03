using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Jacko.MessageBus;

public class MessageBus : IMessageBus
{

    private readonly string connectionString = "Endpoint=sb://js-servbus1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZdpdjnnOCrksuqY7ujwgf+hdFNlZXzWaU+ASbDD77ZQ=";

    public async Task PublishMessage(object message, string topic_queue_name)
    {
        await using var client = new ServiceBusClient(connectionString);

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


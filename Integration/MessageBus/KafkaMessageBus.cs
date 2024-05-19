using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Jacko.MessageBus
{
	public class KafkaMessageBus:IMessageBus
	{
        private readonly string _bootstrapServers;        

        public KafkaMessageBus(string bootstrapServers)
        {
            _bootstrapServers = bootstrapServers;
        }

        public async Task PublishMessage(object message, string topic_queue_name)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var jsonMessage = JsonConvert.SerializeObject(message);
                    
                    var result = await producer.ProduceAsync(topic_queue_name, new Message<Null, string> { Value = jsonMessage });
                    Console.WriteLine($"Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}


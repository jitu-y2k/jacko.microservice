using System;
using System.Threading.Channels;
using Confluent.Kafka;
using Jacko.Services.EmailAPI.Models.Dto;
using Jacko.Services.EmailAPI.Service;
using Newtonsoft.Json;

//using Microsoft.EntityFrameworkCore.Metadata;


namespace Jacko.Services.EmailAPI.Messaging
{
    public class KafkaMQEmailConsumer : BackgroundService
    {
        
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly string _bootstrapServer;
        private readonly string _topic;        

        public KafkaMQEmailConsumer(EmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
            var platform = _configuration["AsyncCommunicationConfig:Platform"] ?? "";

            _topic = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")??"";
            _bootstrapServer = _configuration[$"AsyncCommunicationConfig:{platform}:BootstrapServer"] ?? "";
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServer,
                GroupId = "EmailReceivers",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics=true                
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(cancellationToken);
                        CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(result.Message.Value);                        

                        //Log the message into database
                        _emailService.EmailCartAndLog(cartDto);                           
                        
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
                return Task.CompletedTask;
            }
        }

        
    }
}


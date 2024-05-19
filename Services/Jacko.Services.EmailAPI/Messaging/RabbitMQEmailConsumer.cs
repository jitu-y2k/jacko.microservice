using System;
using System.Text;
using Jacko.Services.EmailAPI.Models.Dto;
using Jacko.Services.EmailAPI.Service;
using Newtonsoft.Json;
//using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Jacko.Services.EmailAPI.Messaging
{
    public class RabbitMQEmailConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        //private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
        private readonly string _queueName;        

        public RabbitMQEmailConsumer(EmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
            var platform = _configuration["AsyncCommunicationConfig:Platform"] ?? "";

            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>($"{platform}:Host"),
                Password = _configuration.GetValue<string>($"{platform}:Password"),
                UserName = _configuration.GetValue<string>($"{platform}:UserId")
            };
            _queueName = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, false, false, false, arguments: null);
            //_channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: false);
            //queueName = _channel.QueueDeclare().QueueName;
            //_channel.QueueBind(queueName, ExchangeName, "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(content);

                try
                {
                    //Log the message into database
                    _emailService.EmailCartAndLog(cartDto);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    throw;
                }
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        
    }
}


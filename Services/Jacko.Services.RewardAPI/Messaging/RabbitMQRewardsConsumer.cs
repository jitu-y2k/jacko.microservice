using System.Text;
using Jacko.Services.RewardAPI.Message;
using Jacko.Services.RewardAPI.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Jacko.Services.RewardAPI.Messaging
{
    public class RabbitMQRewardsConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly RewardService _rewardService;
        private readonly IConfiguration _configuration;
        //private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
        private readonly string _queueName;        

        public RabbitMQRewardsConsumer(RewardService rewardService, IConfiguration configuration)
        {
            _rewardService = rewardService;
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>("RabbitMQServer:Host"),
                Password = _configuration.GetValue<string>("RabbitMQServer:Password"),
                UserName = _configuration.GetValue<string>("RabbitMQServer:UserId")
            };
            _queueName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
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

                RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(content);

                try
                {
                    //Log the message into database
                    _rewardService.UpdateRewards (rewardsMessage);

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


using System;
using Microsoft.Azure.Amqp;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;

namespace Jacko.MessageBus
{
	public class RabbitMQMessageBus: IMessageBus
	{
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _username;
        private IConnection? _connection=null;

        //public string? ConnectionString { get; set; }
        //public string? HostName { get; set; }
        //public string? UserName { get; set; }
        //public string? Password { get; set; }

        public RabbitMQMessageBus(string hostName, string username, string password)
        {
            _hostName = hostName;
            _password = password;
            _username = username;
        }

        public async Task PublishMessage(object baseMessage, string queueName)
        {

            if (ConnectionExists())
            {
                var channel = _connection!.CreateModel();

                channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

                var jsonMessage = JsonConvert.SerializeObject(baseMessage);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                await Task.CompletedTask;
            }
        }

        private void CreateConnection()
        {
            try
            {
                if (_hostName =="" || _username=="" || _password=="")
                {
                    throw new Exception("RabbitMQ conectivity information is not set");
                }

                var factory = new ConnectionFactory()
                {
                    HostName = _hostName,
                    Password = _password,
                    UserName = _username
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                //Log exception if required
            }

        }

        private bool ConnectionExists()
        {
            if (_connection != null)
                return true;

            CreateConnection();

            return _connection != null;
        }
    }
}


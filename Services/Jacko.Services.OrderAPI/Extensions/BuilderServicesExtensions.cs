using System;
using Jacko.MessageBus;

namespace Jacko.Services.OrderAPI.Extensions
{
	public static class BuilderServicesExtensions
	{
		public static WebApplicationBuilder AddAsyncCommunicationService(this WebApplicationBuilder builder)
		{
            var platform = builder.Configuration["AsyncCommunicationConfig:Platform"] ?? "";

            switch (platform.ToLower())
            {
                case "rabbitmq":

                    builder.Services.AddScoped<IMessageBus, RabbitMQMessageBus>(provider =>
                    {
                        var rabbitMQConfig = builder.Configuration.GetSection($"AsyncCommunicationConfig:{platform}");
                        return new RabbitMQMessageBus(rabbitMQConfig.GetValue<string>("Host")!, rabbitMQConfig.GetValue<string>("Username")!, rabbitMQConfig.GetValue<string>("Password")!);
                    });
                    break;
                case "azureservicebus":
                    builder.Services.AddScoped<IMessageBus, AzureServiceMessageBus>(provider =>
                    {
                        var rabbitMQConfig = builder.Configuration.GetSection($"AsyncCommunicationConfig:{platform}");
                        return new AzureServiceMessageBus(rabbitMQConfig.GetValue<string>("ConnectionString")!);
                    });
                    break;
                case "kafka":
                    builder.Services.AddScoped<IMessageBus, KafkaMessageBus>(provider =>
                    {
                        var rabbitMQConfig = builder.Configuration.GetSection($"AsyncCommunicationConfig:{platform}");
                        return new KafkaMessageBus(rabbitMQConfig.GetValue<string>("BootstrapServer")!);
                    });
                    break;
                default:
                    Console.WriteLine("No platform configured for Async communication");
                    break;
            }
            return builder;
		}
	}
}


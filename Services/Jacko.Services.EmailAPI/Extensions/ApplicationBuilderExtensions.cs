using System;
using Jacko.Services.EmailAPI.Messaging;

namespace Jacko.Services.EmailAPI.Extensions
{
	public static class ApplicationBuilderExtensions
	{
        private static IAzureServiceBusConsumer _serviceBusConsumer;

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {

            _serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            Console.WriteLine("Email Service Started ");
            _serviceBusConsumer.Start();
        }

        private static void OnStop()
        {
            _serviceBusConsumer.Stop();
            Console.WriteLine("Email Service Stopped ");
        }

        public static WebApplicationBuilder AddAsyncCommunicationService(this WebApplicationBuilder builder)
        {
            var platform = builder.Configuration["AsyncCommunicationConfig:Platform"] ?? "";

            switch (platform.ToLower())
            {
                case "rabbitmq":

                    builder.Services.AddHostedService<RabbitMQEmailConsumer>();
                    break;
                case "azureservicebus":
                    builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
                    break;
                case "kafka":
                    builder.Services.AddHostedService<KafkaMQEmailConsumer>();
                    break;
                default:
                    Console.WriteLine("No platform configured for Async communication");
                    break;
            }
            return builder;
        }

    }

    
}


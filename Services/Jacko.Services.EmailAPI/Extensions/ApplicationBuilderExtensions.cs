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

    }

    
}


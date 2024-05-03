using System;
namespace Jacko.Services.EmailAPI.Messaging
{
	public interface IAzureServiceBusConsumer
	{
		Task Start();
		Task Stop();
	}
}


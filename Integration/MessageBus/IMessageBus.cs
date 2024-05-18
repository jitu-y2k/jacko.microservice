using System;
namespace Jacko.MessageBus
{
	public interface IMessageBus
	{
		Task PublishMessage(object message, string topic_queue_name);

		public string? ConnectionString { get; set; }
		public string? HostName { get; set; }
		public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}


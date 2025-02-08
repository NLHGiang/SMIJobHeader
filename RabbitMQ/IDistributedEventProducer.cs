namespace SMIJobHeader.RabbitMQ;

public interface IDistributedEventProducer
{
    Task PublishAsync<T>(T message, string topic = null, string subject = null,
        IDictionary<string, string> attributes = null);

    Task PublishMesageAsync<T>(T message, string queueName = null, string exchange = null, string routingKey = null,
        IDictionary<string, string> attributes = null);
}
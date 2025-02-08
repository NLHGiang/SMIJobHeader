using RabbitMQ.Client;

namespace SMIJobHeader.RabbitMQ;

public interface IRabbitMQConnector
{
    IConnection Connection { get; }
}
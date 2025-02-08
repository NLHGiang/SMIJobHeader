using RabbitMQ.Client;
using SMIJobHeader.RabbitMQ;

namespace SMIJobHeader.BL.Interface;

public interface IRabbitETLService
{
    IConnection Connection { get; }
    RabbitMQOption GetConfig();
}
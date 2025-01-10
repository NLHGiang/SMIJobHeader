using RabbitMQ.Client;
using SMIJobHeader.Model.RabbitMQ;

namespace SMIJobHeader.BL.Interface;

public interface IRabbitETLService
{
    IConnection Connection { get; }
    RabbitOption GetConfig();
}
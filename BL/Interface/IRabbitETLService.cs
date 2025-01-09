using RabbitMQ.Client;
using SMIJobXml.Model.RabbitMQ;
namespace SMIJobXml.BL.Interface
{
    public interface IRabbitETLService
    {
        IConnection Connection { get; }
        RabbitOption GetConfig();
    }
}

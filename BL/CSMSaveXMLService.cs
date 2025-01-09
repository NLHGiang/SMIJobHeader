using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SMIJobXml.BL.Interface;
using SMIJobXml.Extensions;

namespace SMIJobXml.BL
{
    public class CSMSaveXMLService : ICSMSaveXMLService, IDisposable
    {
        private readonly IModel _model;
        private readonly IConnection _connection;
        private readonly string? _queueName = "invoice-raw-xml";
        private readonly IXmlService _logService;
        private readonly ILogger<CSMSaveXMLService> _logger;
        public CSMSaveXMLService(
            IRabbitETLService rabbitMqService,
            IXmlService logService,
            ILogger<CSMSaveXMLService> logger)
        {
            _connection = rabbitMqService.Connection;
            var rabbitOption = rabbitMqService.GetConfig();
            _queueName = rabbitOption?.QueueName;
            _model = _connection.CreateModel();
            _model.BasicQos(0, 1, false);
            _model.QueueDeclare(queue: _queueName, exclusive: false, autoDelete: false, durable: true);
            _model.ExchangeDeclare(rabbitOption?.Exchange, ExchangeType.Topic, autoDelete: false, durable: true);
            _model.QueueBind(_queueName, rabbitOption?.Exchange, rabbitOption?.RoutingKey);
            _logService = logService;
            _logger = logger;
        }
        public void Dispose()
        {
            if (_model.IsOpen)
                _model.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }

        public async Task ReadMessgaes()
        {
            var consumer = new AsyncEventingBasicConsumer(_model);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray().ConvertBytesToString();
                    await _logService.DispenseXmlMessage(body);
                    _model.BasicAck(ea.DeliveryTag, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            };
            _model.BasicConsume(_queueName, false, consumer);
            await Task.CompletedTask;

        }
    }
}

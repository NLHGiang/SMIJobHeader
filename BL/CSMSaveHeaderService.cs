using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SMIJobHeader.BL.Interface;
using SMIJobHeader.Extensions;

namespace SMIJobHeader.BL;

public class CSMSaveHeaderService : ICSMSaveHeaderService, IDisposable
{
    private readonly IConnection _connection;
    private readonly ILogger<CSMSaveHeaderService> _logger;
    private readonly IHeaderService _logService;
    private readonly IModel _model;
    private readonly string? _queueName = "invoice-raw-excel";

    public CSMSaveHeaderService(
        IRabbitETLService rabbitMqService,
        IHeaderService logService,
        ILogger<CSMSaveHeaderService> logger)
    {
        _connection = rabbitMqService.Connection;
        var RabbitMQOption = rabbitMqService.GetConfig();
        _queueName = RabbitMQOption?.QueueName;
        _model = _connection.CreateModel();
        _model.BasicQos(0, 1, false);
        _model.QueueDeclare(_queueName, exclusive: false, autoDelete: false, durable: true);
        _model.ExchangeDeclare(RabbitMQOption?.Exchange, ExchangeType.Topic, autoDelete: false, durable: true);
        _model.QueueBind(_queueName, RabbitMQOption?.Exchange, RabbitMQOption?.RoutingKey);
        _logService = logService;
        _logger = logger;
    }

    public async Task ReadMessgaes()
    {
        var consumer = new AsyncEventingBasicConsumer(_model);

        consumer.Received += async (ch, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray().ConvertBytesToString();
                await _logService.DispenseHeaderMessage(body);
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

    public void Dispose()
    {
        if (_model.IsOpen)
            _model.Close();
        if (_connection.IsOpen)
            _connection.Close();
    }
}
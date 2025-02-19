using System.Diagnostics;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace SMIJobHeader.RabbitMQ;

public class BaseEventProducer : IDistributedEventProducer
{
    private static readonly ActivitySource ActivitySource = new(Assembly.GetEntryAssembly().GetName().Name);
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private readonly ILogger<BaseEventProducer> _logger;
    protected readonly IModel Channel;
    protected readonly IConnection Connection;

    public BaseEventProducer(
        IRabbitMQConnector connector,
        ILogger<BaseEventProducer> logger)
    {
        Connection = connector.Connection;
        Channel = connector.Connection.CreateModel();
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, string queueName = null, string exchange = null,
        IDictionary<string, string> attributes = null)
    {
        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#span-name
        using var activity = ActivitySource.StartActivity("SendMessage", ActivityKind.Producer);
        var props = Channel.CreateBasicProperties();
        props.Persistent = true;

        // Depending on Sampling (and whether a listener is registered or not), the
        // activity above may not be created.
        // If it is created, then propagate its context.
        // If it is not created, the propagate the Current context,
        // if any.
        ActivityContext contextToInject = default;
        if (activity != null)
            contextToInject = activity.Context;
        else if (Activity.Current != null)
            contextToInject = Activity.Current.Context;

        // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
        Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), props,
            InjectTraceContextIntoBasicProperties);

        // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
        // See:
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#messaging-attributes
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.rabbitmq.queue", queueName);
        activity?.SetTag("messaging.rabbitmq.exchange", exchange);


        byte[] data;
        if (message.GetType() == typeof(string))
            data = Encoding.UTF8.GetBytes(message.ToString());
        else
            data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        Publish(queueName, exchange, props, data);
        return Task.CompletedTask;
    }

    public Task PublishMesageAsync<T>(T message, string queueName = null, string exchange = null,
        string routingKey = null,
        IDictionary<string, string> attributes = null)
    {
        // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
        // https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#span-name
        using var activity = ActivitySource.StartActivity("SendMessage", ActivityKind.Producer);
        var props = Channel.CreateBasicProperties();
        props.Persistent = true;

        // Depending on Sampling (and whether a listener is registered or not), the
        // activity above may not be created.
        // If it is created, then propagate its context.
        // If it is not created, the propagate the Current context,
        // if any.
        ActivityContext contextToInject = default;
        if (activity != null)
            contextToInject = activity.Context;
        else if (Activity.Current != null)
            contextToInject = Activity.Current.Context;

        // Inject the ActivityContext into the message headers to propagate trace context to the receiving service.
        Propagator.Inject(new PropagationContext(contextToInject, Baggage.Current), props,
            InjectTraceContextIntoBasicProperties);

        // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
        // See:
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/messaging-spans.md#messaging-attributes
        //   * https://github.com/open-telemetry/semantic-conventions/blob/main/docs/messaging/rabbitmq.md
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.rabbitmq.queue", queueName);
        activity?.SetTag("messaging.rabbitmq.exchange", exchange);


        byte[] data;
        if (message.GetType() == typeof(string))
            data = Encoding.UTF8.GetBytes(message.ToString());
        else
            data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        Publish(exchange, routingKey, props, data);
        return Task.CompletedTask;
    }

    public virtual void Publish(string exchange, string routingKey, IBasicProperties props, byte[] data)
    {
        Channel.BasicPublish(string.IsNullOrEmpty(exchange) ? string.Empty : exchange, routingKey, props, data);
    }

    private void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
    {
        try
        {
            if (props.Headers == null)
                props.Headers = new Dictionary<string, object>();

            props.Headers[key] = value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inject trace context.");
        }
    }
}
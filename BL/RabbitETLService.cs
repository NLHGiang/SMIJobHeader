﻿using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SMIJobHeader.BL.Interface;
using SMIJobHeader.Model.Option;
using SMIJobHeader.RabbitMQ;

namespace SMIJobHeader.BL;

public class RabbitETLService : IRabbitETLService
{
    private readonly RabbitMQOption _options;

    public RabbitETLService(IOptions<ETLOption> options)
    {
        _options = options.Value.RabbitHeaderOption;
        var factory = new ConnectionFactory
        {
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        var hosts = _options.Hosts.Select(x => new AmqpTcpEndpoint(x.HostName, x.Port)).ToList();
        Connection = factory.CreateConnection(hosts);
    }

    public IConnection Connection { get; }

    public RabbitMQOption GetConfig()
    {
        return _options;
    }
}
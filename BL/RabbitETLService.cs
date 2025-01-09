﻿using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SMIJobXml.BL.Interface;
using SMIJobXml.Model.Option;
using SMIJobXml.Model.RabbitMQ;

namespace SMIJobXml.BL
{
    public class RabbitETLService : IRabbitETLService
    {
        private readonly RabbitOption _options;
        public RabbitETLService(IOptions<ETLOption> options)
        {
            _options = options.Value.RabbitXMLOption;
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

        public RabbitOption GetConfig()
        {
            return this._options;
        }
    }
}
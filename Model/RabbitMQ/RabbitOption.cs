namespace SMIJobXml.Model.RabbitMQ
{
    public class RabbitOption
    {
        public List<RabbitHostName> Hosts { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? QueueName { get; set; }
        public string? Exchange { get; set; }
        public string? RoutingKey { get; set; }
        public string? VirtualHost { get; set; }
    }

    public class RabbitHostName
    {
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}

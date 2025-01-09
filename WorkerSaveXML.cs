using SMIJobXml.BL.Interface;

namespace SMIJobXml
{
    public class WorkerSaveXML : BackgroundService
    {
        private readonly ICSMSaveXMLService _consumerService;
        public WorkerSaveXML(ICSMSaveXMLService consumerService)
        {
            _consumerService = consumerService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumerService.ReadMessgaes();
        }
    }
}

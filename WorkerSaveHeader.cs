using SMIJobHeader.BL.Interface;

namespace SMIJobHeader;

public class WorkerSaveHeader : BackgroundService
{
    private readonly ICSMSaveHeaderService _consumerService;

    public WorkerSaveHeader(ICSMSaveHeaderService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumerService.ReadMessgaes();
    }
}
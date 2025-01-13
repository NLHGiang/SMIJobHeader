using AutoMapper;
using MongoDB.Driver;
using SMIJobHeader.BL.Interface;
using SMIJobHeader.Common;
using SMIJobHeader.Common.Excel;
using SMIJobHeader.Constants;
using SMIJobHeader.DBAccessor.Interface.Repositories;
using SMIJobHeader.Extensions;
using SMIJobHeader.Model;
using SMIJobHeader.Model.CrawlData;
using SMIJobHeader.Model.Excel;
using SMIJobHeader.Storing;
using static SMIJobHeader.Constants.MinioConstants;

namespace SMIJobHeader.BL;

public class HeaderService : IHeaderService
{
    private readonly IAppUnitOfWork _appUOW;
    private readonly IFileService _fileService;
    private readonly IConfigService _configService;
    private readonly IMapper _mapper;
    private readonly ILogger<HeaderService> _logger;

    public HeaderService(ILogger<HeaderService> logger,
        IAppUnitOfWork appUOW,
        IFileService fileService,
        IConfigService configService,
        IMapper mapper
    )
    {
        _appUOW = appUOW;
        _fileService = fileService;
        _configService = configService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task DispenseHeaderMessage(string messageLog)
    {
        var crawlEInvoice = messageLog.DeserializeObject<RetryCrawl>();

        var excelBytes = Convert.FromBase64String(crawlEInvoice.Result);
        var eInvoiceDtos = await ReadEInvoiceExcel(excelBytes, crawlEInvoice);

        foreach (var dto in eInvoiceDtos)
        {
            (dto.nmmst, dto.nbmst) = crawlEInvoice.InvoiceType switch
            {
                EInvoiceCrawlConstants.PURCHASE => (crawlEInvoice.Username, dto.nbmst),
                EInvoiceCrawlConstants.PURCHASE_SCO => (crawlEInvoice.Username, dto.nbmst),
                EInvoiceCrawlConstants.SOLD => (dto.nmmst, crawlEInvoice.Username),
                EInvoiceCrawlConstants.SOLD_SCO => (dto.nmmst, crawlEInvoice.Username),
                _ => (dto.nmmst, dto.nbmst)
            };

            crawlEInvoice.nbmst = dto.nbmst;
            crawlEInvoice.khhdon = dto.khhdon;
            crawlEInvoice.shdon = dto.shdon;
            crawlEInvoice.khmshdon = dto.khmshdon;
            crawlEInvoice.nmmst = dto.nmmst;
            var result = _mapper.Map<EinvoiceHeader>(dto);
            crawlEInvoice.Result = result.SerializeObjectToString();

            //await PushQueueResultSMIHeader(data.SerializeObjectToString());
        }
    }

    private async Task<List<EInvoiceDto>> ReadEInvoiceExcel(byte[] excelBytes, RetryCrawl retryCrawl)
    {
        List<EInvoiceDto> eInvoiceDtos = new();
        try
        {
            using (var streamData = new MemoryStream())
            {
                await streamData.WriteAsync(excelBytes, 0, excelBytes.Length);
                streamData.Position = 0;
                var excelConfig = await GetConfigColumnImport(RequestType.EINVOICE);
                eInvoiceDtos = await MapExcel.ReadExcel<EInvoiceDto>(streamData, excelConfig, 2, 6);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ReadEInvoiceExcel-EInvoice] {ex.Message}");
        }

        return eInvoiceDtos.Skip(1).ToList();
    }

    private async Task<Dictionary<string, ExcelColumnConfig>> GetConfigColumnImport(string excelConfigType,
    bool isExport = false)
    {
        var excelConfig = await _configService.GetConfigAsync(excelConfigType, "excel");
        var excelColumnConfig = _mapper.Map<List<ExcelColumnConfig>>(excelConfig);
        if (excelColumnConfig == null || excelColumnConfig == null || !excelColumnConfig.Any())
            throw new BusinessLogicException(ResultCode.DataInvalid, "Dữ liệu không hợp lệ hoặc null");
        var result = new Dictionary<string, ExcelColumnConfig>();
        foreach (var objectProperty in excelColumnConfig)
        {
            if (objectProperty.Lable.IsNullOrEmpty() || result.ContainsKey(objectProperty.Lable)) continue;
            if (!isExport && (objectProperty.IsExport ?? false)) continue;
            result.Add(objectProperty.Lable, objectProperty);
        }

        return result;
    }
}
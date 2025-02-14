using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SMIJobHeader.BL.Interface;
using SMIJobHeader.Common;
using SMIJobHeader.Common.Excel;
using SMIJobHeader.Constants;
using SMIJobHeader.DBAccessor.Interface.Repositories;
using SMIJobHeader.Entities;
using SMIJobHeader.Extensions;
using SMIJobHeader.Model;
using SMIJobHeader.Model.CrawlData;
using SMIJobHeader.Model.Excel;
using SMIJobHeader.Model.Option;
using SMIJobHeader.RabbitMQ;
using SMIJobHeader.RestAPI;
using SMIJobHeader.Storing;
using static SMIJobHeader.Constants.MinioConstants;

namespace SMIJobHeader.BL;

public class HeaderService : IHeaderService
{
    private readonly IAppUnitOfWork _appUOW;
    private readonly IFileService _fileService;
    private readonly IConfigService _configService;
    private readonly IDistributedEventProducer _producer;
    private readonly IMapper _mapper;
    private readonly ILogger<HeaderService> _logger;
    private readonly CrawlOption _crawlOption;

    public HeaderService(
        IAppUnitOfWork appUOW,
        IConfigService configService,
        IDistributedEventProducer producer,
        IFileService fileService,
        IMapper mapper,
        ILogger<HeaderService> logger,
        IOptions<CrawlOption> crawlOption
    )
    {
        _appUOW = appUOW;
        _configService = configService;
        _producer = producer;
        _fileService = fileService;
        _mapper = mapper;
        _logger = logger;
        _crawlOption = crawlOption.Value;
    }

    public async Task DispenseHeaderMessage(string messageLog)
    {
        var crawlEInvoice = messageLog.DeserializeObject<CrawlEInvoice>();
        var logCrawl = new LogCrawlDTO();
        var isSuccess = true;
        var errorMessage = string.Empty;

        if (crawlEInvoice.Result.IsNullOrEmpty())
        {
            isSuccess = false;
            errorMessage = "Response is null or empty";
            logCrawl.BuildLogCrawl(crawlEInvoice, isSuccess, errorMessage);
            await PushQueueResultSMILogCrawl(logCrawl.SerializeObjectToString());
            return;
        }

        var excelBytes = Convert.FromBase64String(crawlEInvoice.Result);
        var eInvoiceDtos = await ReadEInvoiceExcel(excelBytes, crawlEInvoice);

        var syncTTXLys = new[] { 5, 6, 8 };
        List<EInvoiceDto> listHeadersSynced = eInvoiceDtos
            .Where(c => syncTTXLys.Contains(c.ttxly))
            .ToList();

        List<invoiceheaders> listHeaders = new();

        foreach (var dto in listHeadersSynced)
        {
            UpdateNmmstAndNbmst(dto, crawlEInvoice);
            UpdateCrawlEInvoiceProperties(dto, crawlEInvoice);

            dto.key = GenerateKey(crawlEInvoice);

            var isHavingDetail = await CheckIfInvoiceRawHavingDetail(dto);
            if (isHavingDetail) continue;

            var isExisted = await CheckIfExistedInvoiceHeader(dto);
            if (isExisted) continue;

            var einvoiceHeader = _mapper.Map<EinvoiceHeader>(dto);


            crawlEInvoice.Result = einvoiceHeader.SerializeObjectToString();
            await PushQueueResultSMIHeader(crawlEInvoice.SerializeObjectToString());

            invoiceheaders header = GetInvoiceHeader(crawlEInvoice, einvoiceHeader);
            listHeaders.Add(header);

            var crawlService = new CrawlApiService(_crawlOption.BaseApiCrawl, _crawlOption.ApiCrawlToken);
            await crawlService.CrawlEInvoice(crawlEInvoice, _crawlOption.UriEinvoice);
        }

        await CreateRangeInvoiceHeader(listHeaders);

        logCrawl.BuildLogCrawl(crawlEInvoice, isSuccess, errorMessage, eInvoiceDtos.Count, listHeadersSynced.Count, listHeaders.Count);
        await PushQueueResultSMILogCrawl(logCrawl.SerializeObjectToString());
    }

    private async Task<bool> CheckIfInvoiceRawHavingDetail(EInvoiceDto dto)
    {
        var repo = _appUOW.GetRepository<invoiceraws, ObjectId>();
        var havingDetail = await repo.Get(Builders<invoiceraws>.Filter.And
        (
            Builders<invoiceraws>.Filter.Eq(c => c.key, dto.key),
            Builders<invoiceraws>.Filter.Ne(c => c.detail_id, null)
        ));

        return havingDetail != null;
    }

    private async Task<bool> CheckIfExistedInvoiceHeader(EInvoiceDto dto)
    {
        var repo = _appUOW.GetRepository<invoiceheaders, ObjectId>();
        var exsitedHeaders = await repo.Get(Builders<invoiceheaders>.Filter.And
        (
            Builders<invoiceheaders>.Filter.Eq(c => c.key, dto.key)
        ));

        return exsitedHeaders != null;
    }

    private invoiceheaders GetInvoiceHeader(CrawlEInvoice crawlEInvoice, EinvoiceHeader einvoiceHeader)
    {
        var header = _mapper.Map<invoiceheaders>(einvoiceHeader);
        header.user = new ObjectId(crawlEInvoice.User);
        header.account = new ObjectId(crawlEInvoice.Account);
        header.from = crawlEInvoice.InvoiceType switch
        {
            EInvoiceCrawlConstants.PURCHASE_SCO or EInvoiceCrawlConstants.SOLD_SCO => "sco-query",
            EInvoiceCrawlConstants.PURCHASE or EInvoiceCrawlConstants.SOLD => "query"
        };
        header.type = crawlEInvoice.InvoiceType switch
        {
            EInvoiceCrawlConstants.PURCHASE or EInvoiceCrawlConstants.PURCHASE_SCO => "purchase",
            EInvoiceCrawlConstants.SOLD or EInvoiceCrawlConstants.SOLD_SCO => "sold"
        };
        return header;
    }

    private void UpdateNmmstAndNbmst(EInvoiceDto dto, CrawlEInvoice crawlEInvoice)
    {
        (dto.nmmst, dto.nbmst) = crawlEInvoice.InvoiceType switch
        {
            EInvoiceCrawlConstants.PURCHASE or EInvoiceCrawlConstants.PURCHASE_SCO =>
                (crawlEInvoice.Username, dto.nbmst),
            EInvoiceCrawlConstants.SOLD or EInvoiceCrawlConstants.SOLD_SCO =>
                (dto.nmmst, crawlEInvoice.Username),
            _ => (dto.nmmst, dto.nbmst)
        };
    }

    private void UpdateCrawlEInvoiceProperties(EInvoiceDto dto, CrawlEInvoice crawlEInvoice)
    {
        crawlEInvoice.nbmst = dto.nbmst;
        crawlEInvoice.khhdon = dto.khhdon;
        crawlEInvoice.shdon = dto.shdon;
        crawlEInvoice.khmshdon = dto.khmshdon;
        crawlEInvoice.nmmst = dto.nmmst;
    }

    private string GenerateKey(CrawlEInvoice crawlEInvoice)
    {
        var invoiceType = GetInvoiceType(crawlEInvoice.InvoiceType);

        return $"{crawlEInvoice.User}_{crawlEInvoice.Account}_{invoiceType}_{crawlEInvoice.nbmst}-{crawlEInvoice.khmshdon}-{crawlEInvoice.khhdon}-{crawlEInvoice.shdon}";
    }

    private string GetInvoiceType(string invoiceType)
    {
        return invoiceType switch
        {
            EInvoiceCrawlConstants.PURCHASE or EInvoiceCrawlConstants.PURCHASE_SCO => "purchase",
            EInvoiceCrawlConstants.SOLD or EInvoiceCrawlConstants.SOLD_SCO => "sold",
            _ => string.Empty
        };
    }

    private async Task CreateRangeInvoiceHeader(List<invoiceheaders> listHeaders)
    {
        if (listHeaders == null || !listHeaders.Any()) return;

        var repo = _appUOW.GetRepository<invoiceheaders, ObjectId>();
        await repo.InsertMany(listHeaders);
    }

    private async Task<List<EInvoiceDto>> ReadEInvoiceExcel(byte[] excelBytes, CrawlEInvoice crawlEInvoice)
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
            var base64String = Convert.ToBase64String(excelBytes);
            _logger.LogError($"[ReadEInvoiceExcel-EInvoice] {ex.Message}\n" +
                base64String);
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

    private async Task PushQueueResultSMIHeader(string result, bool isProduct = true)
    {
        await (isProduct ?
            _producer.PublishMesageAsync(result, "invoice-raw-header", "crawl-invoice-raw",
                "response-invoice-raw-header.*") :
            _producer.PublishMesageAsync(result, "invoice-raw-header-test", "crawl-invoice-raw",
              "response-invoice-raw-header-test.*"));
    }

    private async Task PushQueueResultSMILogCrawl(string result, bool isProduct = true)
    {
        await (isProduct ?
            _producer.PublishMesageAsync(result, "invoice-raw-log-crawl-test", "crawl-invoice-raw",
                "response-invoice-raw-log-crawl-test.*") :
            _producer.PublishMesageAsync(result, "invoice-raw-log-crawl", "crawl-invoice-raw",
                "response-invoice-raw-log-crawl.*"));
    }
}
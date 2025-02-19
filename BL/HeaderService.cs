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
    private const int DETAIL_ERROR_COUNT = 10;
    private readonly IAppUnitOfWork _appUOW;
    private readonly IConfigService _configService;
    private readonly CrawlOption _crawlOption;
    private readonly IFileService _fileService;
    private readonly ILogger<HeaderService> _logger;
    private readonly IMapper _mapper;
    private readonly IDistributedEventProducer _producer;

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
        try
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
                await PushQueueResultSMILogCrawl(logCrawl.SerializeObjectToString(), crawlEInvoice.IsProduct);
                return;
            }

            var excelBytes = Convert.FromBase64String(crawlEInvoice.Result);
            var eInvoiceDtos = await ReadEInvoiceExcel(excelBytes, crawlEInvoice);

            var syncTTXLys = new[] { 5, 6, 8 };
            List<EInvoiceDto> listSyncedHeaders = eInvoiceDtos
                .Where(c => syncTTXLys.Contains(c.ttxly))
                .ToList();

            (var createdCount, List<invoiceheaders> listHeaders) =
                await ProcessListSyncedHeaders(crawlEInvoice, listSyncedHeaders);

            await CreateRangeInvoiceHeader(listHeaders);

            logCrawl.BuildLogCrawl(crawlEInvoice, isSuccess, errorMessage, eInvoiceDtos.Count, listSyncedHeaders.Count,
                createdCount);
            await PushQueueResultSMILogCrawl(logCrawl.SerializeObjectToString(), crawlEInvoice.IsProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.SerializeObjectToString());
        }
    }

    private async Task<(int createdCount, List<invoiceheaders> listHeaders)> ProcessListSyncedHeaders(
        CrawlEInvoice crawlEInvoice, List<EInvoiceDto> listHeadersSynced)
    {
        var createdCount = 0;
        List<invoiceheaders> listHeaders = new();

        foreach (var dto in listHeadersSynced)
        {
            UpdateNmmstAndNbmst(dto, crawlEInvoice);
            UpdateCrawlEInvoiceProperties(dto, crawlEInvoice);

            dto.key = GenerateKey(crawlEInvoice);

            var einvoiceHeader = _mapper.Map<EinvoiceHeader>(dto);
            crawlEInvoice.Result = einvoiceHeader.SerializeObjectToString();

            var (shouldSkip, created) = await ProcessInvoice(dto, crawlEInvoice);
            createdCount += created;

            if (!created.Equals(0))
                await PushQueueResultSMIHeader(crawlEInvoice.SerializeObjectToString(), crawlEInvoice.IsProduct);

            if (shouldSkip) continue;

            var header = GetMappedInvoiceHeader(crawlEInvoice, einvoiceHeader);
            header.run_crawl_detail = DateTime.Now;
            listHeaders.Add(header);

            await RequestCrawlDetail(crawlEInvoice);
        }

        return (createdCount, listHeaders);
    }

    private async Task RequestCrawlDetail(CrawlEInvoice crawlEInvoice)
    {
        var crawlService = new CrawlApiService(_crawlOption.BaseApiCrawl, _crawlOption.ApiCrawlToken);
        await crawlService.CrawlEInvoice(crawlEInvoice, _crawlOption.UriEinvoice);
    }

    private async Task<(bool, int)> ProcessInvoice(EInvoiceDto dto, CrawlEInvoice crawlEInvoice)
    {
        var invoiceraw = await GetInvoiceRaw(dto);
        var isExistedInvoiceRaw = invoiceraw == null;

        var createdCount = isExistedInvoiceRaw ? 1 : 0;

        if (CheckIfInvoiceRawHavingDetail(invoiceraw) || await CheckIfExistedInvoiceHeader(dto))
            return (true, createdCount);

        return (false, createdCount);
    }

    private static bool CheckIfInvoiceRawHavingDetail(invoiceraws? invoiceraw)
    {
        if (invoiceraw == null) return false;

        if (
            invoiceraw.from != null &&
            invoiceraw.type != null &&
            invoiceraw.assets.detail.done == false &&
            invoiceraw.assets.detail.running != null &&
            invoiceraw.assets.detail.error_count < DETAIL_ERROR_COUNT &&
            invoiceraw.assets.detail.run < DateTime.Now) return false;

        return true;
    }

    private async Task<invoiceraws> GetInvoiceRaw(EInvoiceDto dto)
    {
        var repo = _appUOW.GetRepository<invoiceraws, ObjectId>();
        var temp = await repo.Get(
            Builders<invoiceraws>.Filter.And(
                Builders<invoiceraws>.Filter.Eq(c => c.key, dto.key)
            )
        );
        return temp;
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

    private invoiceheaders GetMappedInvoiceHeader(CrawlEInvoice crawlEInvoice, EinvoiceHeader einvoiceHeader)
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
        crawlEInvoice.CrawlType = EInvoiceCrawlConstants.DETAIL;
    }

    private string GenerateKey(CrawlEInvoice crawlEInvoice)
    {
        var invoiceType = GetInvoiceType(crawlEInvoice.InvoiceType);

        return
            $"{crawlEInvoice.User}_{crawlEInvoice.Account}_{invoiceType}_{crawlEInvoice.nbmst}-{crawlEInvoice.khmshdon}-{crawlEInvoice.khhdon}-{crawlEInvoice.shdon}";
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

    private async Task CreateRangeInvoiceHeader(List<invoiceheaders>? listHeaders)
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

    private async Task PushQueueResultSMIHeader(string result, bool isProduct)
    {
        await (isProduct
            ? _producer.PublishMesageAsync(result, "invoice-raw-header", "crawl-invoice-raw",
                "response-invoice-raw-header.*")
            : _producer.PublishMesageAsync(result, "invoice-raw-header-test", "crawl-invoice-raw",
                "response-invoice-raw-header-test.*"));
    }

    private async Task PushQueueResultSMILogCrawl(string result, bool isProduct)
    {
        await (isProduct
            ? _producer.PublishMesageAsync(result, "invoice-raw-log-crawl", "crawl-invoice-raw",
                "response-invoice-raw-log-crawl.*")
            : _producer.PublishMesageAsync(result, "invoice-raw-log-crawl-test", "crawl-invoice-raw",
                "response-invoice-raw-log-crawl-test.*"));
    }
}
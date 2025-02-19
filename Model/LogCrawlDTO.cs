using System.Globalization;
using SMIJobHeader.Constants;
using SMIJobHeader.Model.CrawlData;

namespace SMIJobHeader.Model;

public class LogCrawlDTO
{
    public string? user { get; set; }
    public string? account { get; set; }
    public string? type { get; set; }
    public string? status { get; set; }
    public string? account_username { get; set; }
    public string? description { get; set; }
    public string? error_message { get; set; }
    public string? ge { get; set; }
    public string? le { get; set; }
    public object? latency { get; set; }
    public DateTime created_date { get; set; }
    public int __v { get; set; }
    public int? total { get; set; }
    public int? synced { get; set; }
    public int? created { get; set; }

    public void BuildLogCrawl(CrawlEInvoice crawlEInvoice, bool isSuccess, string errorMessage, int? totalRecord = null,
        int? syncedRecord = null, int? createdRecord = null)
    {
        user = crawlEInvoice.User;
        account = crawlEInvoice.Account;
        account_username = crawlEInvoice.Username;
        created_date = DateTime.Now;
        SetType(crawlEInvoice);
        SetStatusAndDescription(isSuccess, errorMessage);
        SetGeLe(crawlEInvoice);
        SetResultCrawl(totalRecord, syncedRecord, createdRecord);
    }

    private void SetType(CrawlEInvoice crawlEInvoice)
    {
        type = crawlEInvoice.InvoiceType switch
        {
            EInvoiceCrawlConstants.PURCHASE => EInvoiceLogCrawlConstants.PURCHASE,
            EInvoiceCrawlConstants.SOLD => EInvoiceLogCrawlConstants.SOLD,
            EInvoiceCrawlConstants.PURCHASE_SCO => EInvoiceLogCrawlConstants.PURCHASE_SCO,
            EInvoiceCrawlConstants.SOLD_SCO => EInvoiceLogCrawlConstants.SOLD_SCO,
            _ => $"Invalid type [{crawlEInvoice.InvoiceType}]"
        };
    }

    private void SetStatusAndDescription(bool isSuccess, string? errorMessage)
    {
        status = isSuccess ? "Thành công" : "Tải lỗi";
        description = isSuccess ? null : "Lỗi mạng";
        if (!isSuccess) error_message = errorMessage;
    }

    private void SetGeLe(CrawlEInvoice crawlEInvoice)
    {
        var format = "dd/MM/yyyyTHH:mm:ss";
        var provider = CultureInfo.InvariantCulture;

        if (DateTime.TryParseExact(crawlEInvoice.FromDate, format, provider, DateTimeStyles.None, out var geParsedDate))
            ge = geParsedDate.AddHours(-7).ToString("yyyy-MM-ddTHH:mm:ssZ");

        if (DateTime.TryParseExact(crawlEInvoice.ToDate, format, provider, DateTimeStyles.None, out var leParsedDate))
            le = leParsedDate.AddHours(-7).ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    private void SetResultCrawl(int? totalRecord, int? syncedRecord, int? createdRecord)
    {
        total = totalRecord;
        synced = syncedRecord;
        created = createdRecord;
    }
}
namespace SMIJobHeader.Model;

using MongoDB.Bson;
using SMIJobHeader.Constants;
using SMIJobHeader.Model.CrawlData;
using System;
using System.Collections.Generic;

public class LogCrawlDTO
{
    public ObjectId user { get; set; }
    public ObjectId account { get; set; }
    public string? type { get; set; }
    public string? status { get; set; }
    public string? account_username { get; set; }
    public string? description { get; set; }
    public string? error_message { get; set; }
    public string? ge { get; set; }
    public string? le { get; set; }
    public Latency? latency { get; set; }
    public DateTime created_date { get; set; }
    public int __v { get; set; }
    public int total { get; set; }
    public int synced { get; set; }
    public int created { get; set; }

    public void BuildLogCrawl(CrawlEInvoice crawlEInvoice, bool isSuccess, string errorMessage, int totalRecord, int syncedRecord, int createdRecord)
    {
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
        ge = crawlEInvoice.FromDate;
        le = crawlEInvoice.ToDate;
    }

    private void SetResultCrawl(int totalRecord, int syncedRecord, int createdRecord)
    {
        total = totalRecord;
        synced = syncedRecord;
        created = createdRecord;
    }
}

public class Latency
{
    public List<int> latency_sync { get; set; }
    public int total { get; set; }
}
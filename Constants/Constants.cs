namespace SMIJobHeader.Constants;

public static class EInvoiceCrawlConstants
{
    public const string PURCHASE = "purchase";
    public const string SOLD = "sold";
    public const string PURCHASE_SCO = "purchase_sco";
    public const string SOLD_SCO = "sold_sco";

    public const string DETAIL = "detail";
    public const string XML = "xml";
    public static readonly List<string> InvoiceTypes = new() { PURCHASE, PURCHASE_SCO, SOLD, SOLD_SCO };
    public static readonly List<string> CrawlTypes = new() { DETAIL, XML };
}

public static class FileExtension
{
    public const string PDF = ".pdf";
    public const string XML = ".xml";
    public const string JSON = ".json";
    public const string EXCEL = ".xlsx";
}

public class DateTimeZone
{
    public const string SeAsiaStardTime = "SE Asia Standard Time";
}
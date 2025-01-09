using Newtonsoft.Json;

namespace SMIJobXml.Model.CrawlData;

public class RetryCrawl
{
    [JsonProperty("user")]
    public string User { get; set; }

    [JsonProperty("account")]
    public string Account { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("fromDate")]
    public string? FromDate { get; set; }

    [JsonProperty("toDate")]
    public string? ToDate { get; set; }

    [JsonProperty("invoiceType")]
    public string InvoiceType { get; set; }

    [JsonProperty("crawlType")]
    public string CrawlType { get; set; } // 'detail', 'xml'

    [JsonProperty("nbmst")]
    public string? nbmst { get; set; }

    [JsonProperty("nmmst")]
    public string? nmmst { get; set; }

    [JsonProperty("khmshdon")]
    public string? khmshdon { get; set; }

    [JsonProperty("shdon")]
    public string? shdon { get; set; }

    [JsonProperty("khhdon")]
    public string? khhdon { get; set; }

    [JsonProperty("result")]
    public string? Result { get; set; }

    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; } = true;

    [JsonProperty("minutesAppendCrawlError")]
    public int MinutesAppendCrawlError { get; set; }
}
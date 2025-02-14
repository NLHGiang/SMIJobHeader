namespace SMIJobHeader.Model.CrawlData;

public class ResponseDto
{
    public bool IsError { get; set; }
    public string? Result { get; set; }
    public string? ClientId { get; set; }
    public RequestDto Request { get; set; }
}

public class RequestDto
{
    public string? Method { get; set; }
    public string? ResponseType { get; set; }
    public string? BaseApi { get; set; }
    public string? Cookies { get; set; }
    public string? Token { get; set; }
    public string? ApiUrl { get; set; }
    public string? ApiPushResult { get; set; }
    public string? ApiTokenPushResult { get; set; }
    public string? FunctionName { get; set; }
    public int Step { get; set; }
    public string? ParameterRequest { get; set; }
    public int NumberRetry { get; set; }

    /// <summary>
    ///     Là step trước khi crawl lại dữ liệu
    /// </summary>
    public int? PastStep { get; set; }

    /// <summary>
    ///     Id of bot crawl data
    /// </summary>
    public string? ClientId { get; set; }

    public int TimeDelay { get; set; }
    public CrawlHeader Headers { get; set; }
}

public class CrawlHeader
{
    public string? Authorization { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
    public Dictionary<string, string>? FormData { get; set; }
}
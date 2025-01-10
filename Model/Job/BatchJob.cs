namespace SMIJobHeader.Model.Job;

public class BatchJob
{
    public string? UrlApi { get; set; } = "https://api-tracuu.minvoice.com.vn/";
    public string? Token { get; set; } = "minvoice@123abc";
    public string? Type { get; set; } = "Error";
    public string? FunctionName { get; set; } = "TaxPlayer";
    public DateTime? LastSync { get; set; }
    public int TimeAppend { get; set; } = -10;
    public string? CronExpression { get; set; } = "*/30 1-10 * * *";
}
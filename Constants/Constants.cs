namespace SMIJobHeader.Constants;

public static class CommonConstants
{
    public const string DefaultCulture = "vi-VN";
    public const char CommaChar = ',';
    public const char SemicolonChar = ';';
}

public static class EInvoiceCrawlConstants
{
    private const string URI_EINVOICE = "/api/v1/crawl/einvoice";
    private const string BASE_API_CRAWL = "https://api-tracuu.minvoice.com.vn/";
    private const string API_CRAWL_TOKEN = "minvoice";

    public const string PURCHASE = "purchase";
    public const string SOLD = "sold";
    public const string PURCHASE_SCO = "purchase_sco";
    public const string SOLD_SCO = "sold_sco";

    public const string EXCEL = "excel";
    public const string DETAIL = "detail";
    public const string XML = "xml";
    public static readonly List<string> InvoiceTypes = new() { PURCHASE, PURCHASE_SCO, SOLD, SOLD_SCO };
    public static readonly List<string> CrawlTypes = new() { EXCEL, DETAIL, XML };

    public static readonly Dictionary<string, int> EInvoiceStatusMapping = new()
    {
        { "Hóa đơn mới", 1 },
        { "Hóa đơn thay thế", 2 },
        { "Hóa đơn điều chỉnh", 3 },
        { "Hóa đơn đã bị thay thế", 4 },
        { "Hóa đơn đã bị điều chỉnh", 5 },
        { "Hóa đơn hủy", 6 }
    };

    public static readonly Dictionary<string, int> EInvoiceResultMapping = new()
    {
        { "Tổng cục thuế đã nhận", 0 },
        { "Đang tiến hành kiểm tra điều kiện cấp mã", 1 },
        { "CQT từ chối hóa đơn theo từng lần phát sinh", 2 },
        { "Hóa đơn đủ điều kiện cấp mã", 3 },
        { "Hóa đơn không đủ điều kiện cấp mã", 4 },
        { "Đã cấp mã hóa đơn", 5 },
        { "Tổng cục thuế đã nhận không mã", 6 },
        { "Đã kiểm tra định kỳ HĐĐT không có mã", 7 },
        { "Tổng cục thuế đã nhận hóa đơn có mã khởi tạo từ máy tính tiền", 8 }
    };
}

public static class EInvoiceLogCrawlConstants
{
    public const string PURCHASE = "sync_invoices_purchase";
    public const string SOLD = "sync_invoices_sold";
    public const string PURCHASE_SCO = "sync_invoices_purchase_sco";
    public const string SOLD_SCO = "sync_invoices_sold_sco";
}

public static class FileExtension
{
    public const string PDF = ".pdf";
    public const string JSON = ".json";
    public const string EXCEL = ".xlsx";
}

public static class ExcelSheet
{
    public const string SheetDefault = "Sheet 1";
}

public static class ErrorMessageValid
{
    public const string MsgRequired = "Dòng số {1}, {0} là bắt buộc nhập";
    public const string MsgMinimumLength = "Dòng số {2}, {0} chiều dài tối thiểu phải là {1}";
    public const string MsgMaxLength = "Dòng số {2}, {0} phải có chiều dài tối đa là {1}";
    public const string MsgMinimumValue = "Dòng số {2}, {0} giá trị tối thiểu phải là {1}";
    public const string MsgMaxValue = "{0} giá trị tối đa phải là {1}";
    public const string MsgPropertyNotFound = "Là {0} thuộc tính không được tìm thấy";
    public const string MsgDatatypeInvalid = "Loại dữ liệu nguồn là định dạng không hợp lệ của {0}";
}

public static class DATATYPE
{
    public const string DOUBLE = "double";
    public const string DECIMAL = "decimal";
    public const string DATETIME = "datetime";
    public const string BOOLEAN = "boolean";
    public const string STRING = "string";
    public const string INT32 = "int32";
    public const string INT = "int";
    public const string GUID = "guid";
    public const string LIST = "list";
    public static readonly List<string> TypeCheckMinMaxLenght = new() { STRING };
    public static readonly List<string> TypeCheckMinMaxValue = new() { DOUBLE, DECIMAL, INT32 };
    public static readonly List<string> TypeNumber = new() { DOUBLE, DECIMAL, INT32, INT };
}

public class DateTimeZone
{
    public const string SeAsiaStardTime = "SE Asia Standard Time";
}
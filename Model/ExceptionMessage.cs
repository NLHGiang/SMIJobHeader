using SMIJobHeader.Common;

namespace SMIJobHeader.Model;

public class ExceptionMessage
{
    public ResultCode ErrorCode { get; set; }
    public string? Message { get; set; }
}
using SMIJobXml.Common;

namespace SMIJobXml.Model
{
    public class ExceptionMessage
    {
        public ResultCode ErrorCode { get; set; }
        public string? Message { get; set; }
    }
}

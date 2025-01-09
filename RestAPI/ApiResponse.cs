using SMIJobXml.Common;

namespace SMIJobXml.RestAPI
{
    public class ApiResponse
    {
        public ResultCode Code { get; set; }
        public string? Message { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
        where T : class
    {
        public T Data { get; set; }
    }

    public class ApiResponseList<T> : ApiResponse
        where T : class
    {
        public List<T> Data { get; set; }
    }
}

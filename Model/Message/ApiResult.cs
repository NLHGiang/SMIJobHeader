using Newtonsoft.Json;

namespace SMIJobXml.Model.Message
{
    public abstract class ApiResultBase
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("responseMessage")]
        public string ResponseMessage { get; set; }

        public ApiResultBase() { }

        public ApiResultBase(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
    public class ApiResult : ApiResultBase
    {
        [JsonProperty("error")]
        public Error Error;

        public ApiResult()
            : base()
        {
        }

        public ApiResult(string code, string message)
            : base(code, message)
        {
        }
    }

    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class ApiResult<T> : ApiResultBase
        where T : class
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        public ApiResult()
            : base()
        {

        }
        public ApiResult(string code, string message, T data)
            : base(code, message)
        {
            this.Data = data;
        }

        #region Will be deleted
        public ApiResult(string code, string message)
            : base(code, message)
        {
            this.Code = code;
            this.Message = message;
        }

        public void SetInfo(string code, string message, T data)
        {
            Code = code;
            Message = message;
            this.Data = data;
        }


        public void SetInfo(string code, string message)
        {
            Code = code;
            Message = message;
        }
        #endregion

    }

    public class ApiResultList<T> : ApiResultBase
        where T : class
    {
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }

        public ApiResultList()
            : base()
        {

        }
        public ApiResultList(string code, string message, IEnumerable<T> data)
            : base(code, message)
        {
            this.Data = data;
        }
    }
}

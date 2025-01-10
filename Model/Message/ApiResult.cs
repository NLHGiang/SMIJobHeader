using Newtonsoft.Json;

namespace SMIJobHeader.Model.Message;

public abstract class ApiResultBase
{
    public ApiResultBase()
    {
    }

    public ApiResultBase(string code, string message)
    {
        Code = code;
        Message = message;
    }

    [JsonProperty("code")] public string Code { get; set; }

    [JsonProperty("errorCode")] public string ErrorCode { get; set; }

    [JsonProperty("message")] public string Message { get; set; }

    [JsonProperty("responseMessage")] public string ResponseMessage { get; set; }
}

public class ApiResult : ApiResultBase
{
    [JsonProperty("error")] public Error Error;

    public ApiResult()
    {
    }

    public ApiResult(string code, string message)
        : base(code, message)
    {
    }
}

public class Error
{
    [JsonProperty("code")] public string Code { get; set; }

    [JsonProperty("message")] public string Message { get; set; }
}

public class ApiResult<T> : ApiResultBase
    where T : class
{
    public ApiResult()
    {
    }

    public ApiResult(string code, string message, T data)
        : base(code, message)
    {
        Data = data;
    }

    [JsonProperty("data")] public T Data { get; set; }

    #region Will be deleted

    public ApiResult(string code, string message)
        : base(code, message)
    {
        Code = code;
        Message = message;
    }

    public void SetInfo(string code, string message, T data)
    {
        Code = code;
        Message = message;
        Data = data;
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
    public ApiResultList()
    {
    }

    public ApiResultList(string code, string message, IEnumerable<T> data)
        : base(code, message)
    {
        Data = data;
    }

    [JsonProperty("data")] public IEnumerable<T> Data { get; set; }
}
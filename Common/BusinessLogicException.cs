namespace SMIJobHeader.Common;

public class BusinessLogicException : Exception
{
    public BusinessLogicException()
    {
        ErrorCode = ResultCode.NoError;
    }

    public BusinessLogicException(ResultCode errorCode)
    {
        ErrorCode = errorCode;
    }

    public BusinessLogicException(ResultCode errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessLogicException(ResultCode errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public ResultCode ErrorCode { get; private set; }
}
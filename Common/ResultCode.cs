namespace SMIJobHeader.Common;

public enum ResultCode
{
    NoErrorGetWay = 0,
    NoError = 1,
    DataInvalid,
    NotFound,
    NotBotActive,
    ExistedSerial,
    UnknownError = 99
}
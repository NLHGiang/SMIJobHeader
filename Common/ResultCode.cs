namespace SMIJobXml.Common
{
    public enum ResultCode : int
    {
        NoErrorGetWay = 0,
        NoError = 1,
        DataInvalid,
        NotFound,
        NotBotActive,
        ExistedSerial,
        UnknownError = 99,
    }
}

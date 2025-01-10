namespace SMIJobHeader.BL.Interface;

public interface IHeaderService
{
    Task DispenseXmlMessage(string messageLog);
}
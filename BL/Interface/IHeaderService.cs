namespace SMIJobHeader.BL.Interface;

public interface IHeaderService
{
    Task DispenseHeaderMessage(string messageLog);
}
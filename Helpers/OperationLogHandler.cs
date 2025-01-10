namespace SMIJobHeader.Helpers;

public class OperationLogHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage,
        CancellationToken cancellationToken)
    {
        return base.SendAsync(requestMessage, cancellationToken);
    }
}
namespace SMIJobXml.Storing.Minio;

public class MinioHttpClient : IMinioHttpClient
{
    private readonly HttpClient _client;

    public MinioHttpClient()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => { return true; };
        _client = new HttpClient(handler);
    }

    public HttpClient GetClient()
    {
        return _client;
    }
}
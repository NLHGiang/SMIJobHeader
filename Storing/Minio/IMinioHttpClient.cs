namespace SMIJobHeader.Storing.Minio;

/// <summary>
///     HttpClient support MinIO SDK old version
/// </summary>
public interface IMinioHttpClient
{
    HttpClient GetClient();
}
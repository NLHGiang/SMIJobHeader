namespace SMIJobXml.Storing;

public class ObjectStorageOption
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public bool UseSsl { get; set; }
}
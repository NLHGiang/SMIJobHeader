using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace SMIJobHeader.Storing.Minio;

public class MinioService : IFileService
{
    private readonly MinioClient _minio;
    private readonly ObjectStorageOption _options;

    public MinioService(
        IMinioHttpClient minioHttpClient,
        IOptions<ObjectStorageOption> options)
    {
        _options = options.Value;

        _minio = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey);

        if (_options.UseSsl)
            _minio = _minio.WithSSL();

        _minio = _minio
            .WithHttpClient(minioHttpClient.GetClient())
            .Build();
    }

    public virtual async Task<byte[]> DownloadAsync(string bucket, string fileName)
    {
        try
        {
            var args1 = new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName);
            await _minio.StatObjectAsync(args1);

            var memoryStream = new MemoryStream();
            var args2 = new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName)
                .WithCallbackStream(stream => { stream.CopyTo(memoryStream); });
            await _minio.GetObjectAsync(args2);
            var bytes = memoryStream.ToArray();
            return bytes;
        }
        catch
        {
            return null;
        }
    }

    public virtual async Task UploadAsync(string bucket, string fileName, byte[] bytes)
    {
        try
        {
            var stream = new MemoryStream(bytes);

            // Specify SSE-C encryption options
            // Aes aesEncryption = Aes.Create();
            // aesEncryption.KeySize = 256;
            // aesEncryption.GenerateKey();
            // var ssec = new SSEC(aesEncryption.Key);

            var args = new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(fileName)
                .WithObjectSize(stream.Length)
                .WithStreamData(stream)
                .WithContentType("application/actet-stream");
            await _minio.PutObjectAsync(args);
        }
        catch (ConnectionException)
        {
            await ReUpload(bucket, fileName, bytes);
        }
        catch (UnexpectedMinioException ex)
        {
            if (ex.ServerMessage != "The specified bucket does not exist")
                throw;

            await ReUpload(bucket, fileName, bytes);
        }
        catch (BucketNotFoundException)
        {
            await ReUpload(bucket, fileName, bytes);
        }
    }

    private async Task ReUpload(string bucket, string fileName, byte[] bytes)
    {
        var args = new MakeBucketArgs()
            .WithBucket(bucket);
        await _minio.MakeBucketAsync(args);

        // Upload a file to bucket.
        await UploadAsync(bucket, fileName, bytes);
    }
}
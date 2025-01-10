namespace SMIJobHeader.Storing;

/// <summary>
///     The interface abstracts file handling functions
/// </summary>
public interface IFileService
{
    /// <summary>
    ///     Download file to bytes
    /// </summary>
    /// <param name="bucket">File storage directory</param>
    /// <param name="fileName">Path file</param>
    /// <returns></returns>
    Task<byte[]> DownloadAsync(string bucket, string fileName);

    /// <summary>
    ///     Upload file
    /// </summary>
    /// <param name="bucket">File storage directory</param>
    /// <param name="fileName">Path to save file</param>
    /// <param name="bytes">Data of file</param>
    /// <returns></returns>
    Task UploadAsync(string bucket, string fileName, byte[] bytes);
}
using System.Text;
using System.Text.Json;
using SMIJobHeader.BL.Interface;
using SMIJobHeader.Constants;
using SMIJobHeader.Extensions;
using SMIJobHeader.Model.Config;
using SMIJobHeader.Storing;

namespace SMIJobHeader.BL;

public class ConfigService : IConfigService
{
    private readonly IFileService _fileService;

    public ConfigService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<List<PropertyConfig>> GetConfigAsync(string name, string type)
    {
        var minioFilePath = MinioConstants.GetSettingPath(
            type, name, FileExtension.JSON);

        var bytes = await _fileService.DownloadAsync(MinioConstants.BUCKET, $"{minioFilePath}");
        var jsonString = Encoding.UTF8.GetString(bytes);
        var propertyConfigs = jsonString.DeserializeObject<List<PropertyConfig>>();

        return propertyConfigs.Where(c => c.Active).ToList();
    }

    public async Task<List<PropertyConfig>> GetConfigExcel(string fileName)
    {
        var minioFilePath = $"host/setting/excel/{fileName}";

        var bytes = await _fileService.DownloadAsync(MinioConstants.BUCKET, $"{minioFilePath}");
        var jsonString = Encoding.UTF8.GetString(bytes);
        var propertyConfigs = JsonSerializer.Deserialize<List<PropertyConfig>>(jsonString);

        return propertyConfigs.Where(c => c.Active).ToList();
    }

    public async Task UpdateAsync(string name, string type, List<PropertyConfig> request)
    {
        var minioFilePath = MinioConstants.GetSettingPath(
            type, name, FileExtension.JSON);

        var jsonString = JsonSerializer.Serialize(request);
        var bytes = Encoding.UTF8.GetBytes(jsonString);

        await _fileService.UploadAsync(MinioConstants.BUCKET, $"{minioFilePath}", bytes);
    }
}
using SMIJobHeader.Model.Config;

namespace SMIJobHeader.BL.Interface;

public interface IConfigService
{
    Task<List<PropertyConfig>> GetConfigAsync(string name, string type);
    Task UpdateAsync(string name, string type, List<PropertyConfig> request);

    Task<List<PropertyConfig>> GetConfigExcel(string fileName);
}
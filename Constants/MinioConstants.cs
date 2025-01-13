namespace SMIJobHeader.Constants;

public static class MinioConstants
{
    public static readonly string BUCKET = "asset";
    public static readonly string SETTING_FORMAT = "host/setting/{0}/{1}{2}{3}";

    public static string GetSettingPath(string configType, string name, string fileExtension)
    {
        return string.Format(SETTING_FORMAT, configType, name, ConfigType.ConfigTypeDict[configType], fileExtension);
    }

    public class ConfigType
    {
        public static Dictionary<string, string> ConfigTypeDict = new()
        {
            { "html", "_html_config" },
            { "xml", "_xml_config" },
            { "excel", "_excel_config" },
            { "display", "_display_config" }
        };
    }

    public class RequestType
    {
        public const string EINVOICE = "einvoice";
    }
}
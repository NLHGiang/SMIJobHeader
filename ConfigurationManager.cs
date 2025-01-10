namespace SMIJobHeader;

internal static class ConfigurationManager
{
    static ConfigurationManager()
    {
        AppSetting = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public static IConfiguration AppSetting { get; }
}
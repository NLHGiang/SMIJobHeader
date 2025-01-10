namespace SMIJobHeader.Helpers;

public static class ConfigurationManagerHelpers
{
    static ConfigurationManagerHelpers()
    {
        AppSetting = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>();
    }

    public static IConfiguration AppSetting { get; }
    public static ILoggerFactory Logger { get; }
}
using MongoDB.Driver;
using SMIJobHeader.BL;
using SMIJobHeader.BL.Interface;
using SMIJobHeader.DBAccessor;
using SMIJobHeader.DBAccessor.Interface.Repositories;
using SMIJobHeader.Model.Option;
using SMIJobHeader.RabbitMQ;

namespace SMIJobHeader.ServiceExtension;

public static class ServiceExtension
{
    public static IServiceCollection AddIServicesScoped(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOption = new DbOption();
        configuration.GetSection("DbOption").Bind(dbOption);

        services.AddSingleton<IMongoClient>(serviceProvider => { return new MongoClient(dbOption.ConnectionString); });

        services.AddScoped(serviceProvider =>
        {
            var mongoClient = serviceProvider.GetService<IMongoClient>();
            return mongoClient!.GetDatabase(dbOption.DbName);
        });

        services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        services.AddSingleton<IHeaderService, HeaderService>();
        services.AddSingleton<ICSMSaveHeaderService, CSMSaveHeaderService>();
        services.AddSingleton<IRabbitETLService, RabbitETLService>();

        var options = new RabbitMQOption();
        configuration.GetSection("ETLOption:RabbitHeaderOption").Bind(options);

        services.Configure<RabbitMQOption>(config =>
        {
            config.Hosts = options.Hosts;
            config.Password = options.Password;
            config.UserName = options.UserName;
            config.VirtualHost = options.VirtualHost;
        });

        services.AddSingleton<IRabbitMQConnector, RabbitMQConnector>();
        services.AddSingleton<IDistributedEventProducer, BaseEventProducer>();

        services.AddSingleton<RedisCacheService>();
        services.AddHostedService<WorkerSaveHeader>();
        return services;
    }
}
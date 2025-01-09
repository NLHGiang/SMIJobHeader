using MongoDB.Driver;
using SMIJobXml.BL;
using SMIJobXml.BL.Interface;
using SMIJobXml.DBAccessor;
using SMIJobXml.DBAccessor.Interface.Repositories;
using SMIJobXml.Model.Option;

namespace SMIJobXml.ServiceExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddIServicesScoped(this IServiceCollection services, IConfiguration configuration)
        {
            var dbOption = new DbOption();
            configuration.GetSection("DbOption").Bind(dbOption);

            services.AddSingleton<IMongoClient>(serviceProvider => { return new MongoClient(dbOption.ConnectionString); });

            services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var mongoClient = serviceProvider.GetService<IMongoClient>();
                return mongoClient!.GetDatabase(dbOption.DbName);
            });

            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
            services.AddSingleton<IXmlService, XmlService>();
            services.AddSingleton<ICSMSaveXMLService, CSMSaveXMLService>();
            services.AddSingleton<IRabbitETLService, RabbitETLService>();


            services.AddSingleton<RedisCacheService>();
            services.AddHostedService<WorkerSaveXML>();
            return services;
        }
    }
}

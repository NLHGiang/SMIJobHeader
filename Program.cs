using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SMIJobHeader.Helpers;
using SMIJobHeader.Model;
using SMIJobHeader.Model.Option;
using SMIJobHeader.ServiceExtension;
using SMIJobHeader.Storing;
using SMIJobHeader.Storing.Minio;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService(options => { options.ServiceName = "SMIJobXml"; });
builder.Services.Configure<ObjectStorageOption>(builder.Configuration.GetSection("MinioOption"));
builder.Services.AddScoped<IMinioHttpClient, MinioHttpClient>();
builder.Services.AddScoped<IFileService, MinioService>();
builder.Services.AddIServicesScoped(builder.Configuration);
//
// Add services to the container.
var _config = new ConfigurationBuilder()
    .AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "serilog.config.json", true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(_config, "Serilog")
    .MinimumLevel.Is(LogEventLevel.Verbose)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
        .WriteTo.File(new CompactJsonFormatter(), $"{AppContext.BaseDirectory}/Logs/Information/information_.log",
            retainedFileCountLimit: 3, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning)
        .WriteTo.File(new CompactJsonFormatter(), $"{AppContext.BaseDirectory}/Logs/Warning/warning_.log",
            retainedFileCountLimit: 3, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
        .WriteTo.File(new CompactJsonFormatter(), $"{AppContext.BaseDirectory}/Logs/Error/error_.log",
            retainedFileCountLimit: 3, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Debug)
        .WriteTo.File(new CompactJsonFormatter(), $"{AppContext.BaseDirectory}/Logs/Debug/debug_.log",
            retainedFileCountLimit: 3, rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Verbose)
        .WriteTo.File(new CompactJsonFormatter(), $"{AppContext.BaseDirectory}/Logs/Verbose/verbose_.log",
            retainedFileCountLimit: 3, rollingInterval: RollingInterval.Day))
    .CreateLogger();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseWindowsService().UseSerilog();

builder.Services.Configure<ETLOption>(builder.Configuration.GetSection("ETLOption"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("appsettings"));


builder.Services.AddAuthentication(p =>
{
    p.AddScheme<BasicAuthenticationHandler>("Basic", null);
    p.DefaultAuthenticateScheme = "Basic";
    p.DefaultChallengeScheme = "Basic";
});
builder.Services.AddTransient<OperationLogHandler>();
//var startup = new Startup(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
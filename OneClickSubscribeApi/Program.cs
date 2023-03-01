using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OneClickSubscribeApi.Options;
using OneClickSubscribeApi.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile($"local.settings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
        config.AddUserSecrets<OneClickSubscribeApi.OneClickSubscribeFunction>();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices(services =>
    {
        services.AddOptions<StorageOptions>().Configure<IConfiguration>(
                       (options, config) => config.GetSection(StorageOptions.Key).Bind(options));

        services.AddOptions<ApplicationOptions>().Configure<IConfiguration>(
                       (options, config) => config.GetSection(ApplicationOptions.Key).Bind(options));

        services.AddScoped(provider =>
        {
            var options = provider.GetRequiredService<IOptions<StorageOptions>>().Value;

            return new TableClient(options.TableStorageConnectionString, options.TableName);
        });

        services.AddTransient<IStorageService, StorageService>();
    }) 
    .Build();

await host.RunAsync();
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OneClickSubscribeApi.Domain;
using OneClickSubscribeApi.Domain.Options;
using OneClickSubscribeApi.Infrastructure.Http;
using OneClickSubscribeApi.Infrastructure.Http.Options;
using OneClickSubscribeApi.Infrastructure.Storage;
using OneClickSubscribeApi.Infrastructure.Storage.Options;
using OneClickSubscribeApi.Options;

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
        services.AddOptions<ApplicationOptions>().Configure<IConfiguration>(
                       (options, config) => config.GetSection(nameof(ApplicationOptions)).Bind(options));

        services.AddOptions<SubscriptionOptions>().Configure<IConfiguration>(
            (options, config) => config.GetSection(nameof(SubscriptionOptions)).Bind(options));

        services.AddOptions<HttpOptions>().Configure<IConfiguration>(
            (options, config) => config.GetSection(nameof(HttpOptions)).Bind(options));

        services.AddOptions<StorageOptions>().Configure<IConfiguration>(
            (options, config) => config.GetSection(nameof(StorageOptions)).Bind(options));

        services.AddDomain((provider, options) =>
        {
            var subscriptionOptions = provider.GetRequiredService<IOptions<SubscriptionOptions>>().Value;

            options.ValidTypes = subscriptionOptions.ValidTypes;
            options.DefaultType = subscriptionOptions.DefaultType;
        });

        services.AddHttpInfrastructure((provider, options) =>
        {
            var httpOptions = provider.GetRequiredService<IOptions<HttpOptions>>().Value;

            options.MailchimpApiKey = httpOptions.MailchimpApiKey;
            options.MailchimpApiBaseUrl = httpOptions.MailchimpApiBaseUrl;
            options.MailchimpAudienceId = httpOptions.MailchimpAudienceId;
            options.MailchimpTypeMergeTag = httpOptions.MailchimpTypeMergeTag;
        });

        services.AddStorageInfrastructure((provider, options) =>
        {
            var storageOptions = provider.GetRequiredService<IOptions<StorageOptions>>().Value;

            options.TableName = storageOptions.TableName;
            options.TableStorageConnectionString = storageOptions.TableStorageConnectionString;
        });
    }) 
    .Build();

await host.RunAsync();
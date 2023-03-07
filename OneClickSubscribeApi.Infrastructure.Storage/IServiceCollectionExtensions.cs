using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using OneClickSubscribeApi.Domain.Repositories;
using OneClickSubscribeApi.Infrastructure.Storage.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OneClickSubscribeApi.Infrastructure.Storage.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 
namespace OneClickSubscribeApi.Infrastructure.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageInfrastructure(this IServiceCollection services, Action<IServiceProvider, StorageOptions> configureOptions)
    {
        services.AddTransient(provider =>
        {
            var options = new StorageOptions();
            configureOptions.Invoke(provider, options);
            return options;
        });

        services.AddScoped<ISubscriberRepository, SubscriberRepository>();

        services.AddScoped(provider =>
        {
            var options = provider.GetRequiredService<StorageOptions>();

            return new TableClient(options.TableStorageConnectionString, options.TableName);
        });

        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}
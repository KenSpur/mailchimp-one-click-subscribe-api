using Microsoft.Extensions.DependencyInjection;
using OneClickSubscribeApi.Domain.Options;
using OneClickSubscribeApi.Domain.Services;
using OneClickSubscribeApi.Domain.Services.Implementations;

namespace OneClickSubscribeApi.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services, Action<IServiceProvider, SubscriptionOptions> configureOptions)
    {
        services.AddTransient(provider =>
        {
            var options = new SubscriptionOptions();
            configureOptions.Invoke(provider, options);
            return options;
        });

        services.AddTransient<ISubscriptionProcessingService, SubscriptionProcessingService>();

        return services;
    }
}
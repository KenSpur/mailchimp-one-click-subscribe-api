using Microsoft.Extensions.DependencyInjection;
using OneClickSubscribeApi.Domain.Services;
using OneClickSubscribeApi.Infrastructure.Http.Options;
using System.Text;

namespace OneClickSubscribeApi.Infrastructure.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpInfrastructure(this IServiceCollection services, Action<IServiceProvider, HttpOptions> configureOptions)
    {
        services.AddTransient(provider =>
        {
            var options = new HttpOptions();
            configureOptions.Invoke(provider, options);
            return options;
        });

        services.AddHttpClient<IMailchimpService, MailchimpService>(nameof(MailchimpService), (provider, client) =>
        {
            var options = provider.GetRequiredService<HttpOptions>();
            client.BaseAddress = new Uri($"https://{options.MailchimpApiBaseUrl}");

            var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"key:{options.MailchimpApiKey}"));
            client.DefaultRequestHeaders.Add("Authorization", @$"Basic {basicAuthValue}");
        });

        return services;
    }
}
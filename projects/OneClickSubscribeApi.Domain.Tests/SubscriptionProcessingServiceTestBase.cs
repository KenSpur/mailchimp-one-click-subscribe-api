using OneClickSubscribeApi.Domain.Options;
using OneClickSubscribeApi.Domain.Repositories;
using OneClickSubscribeApi.Domain.Services.Implementations;
using OneClickSubscribeApi.Domain.Services;

namespace OneClickSubscribeApi.Domain.Tests;

public abstract class SubscriptionProcessingServiceTestBase
{
    internal static SubscriptionProcessingService CreateSubscriptionProcessingService(
        SubscriptionOptions? options = null,
        ISubscriberRepository? repository = null,
        IMailchimpService? mailchimpService = null)
    {
        options ??= new SubscriptionOptions();
        repository ??= new Mock<ISubscriberRepository>().Object;
        mailchimpService ??= new Mock<IMailchimpService>().Object;
        return new SubscriptionProcessingService(options, repository, mailchimpService);
    }
}
using Microsoft.Azure.Functions.Worker;
using OneClickSubscribeApi.Domain.Services;

namespace OneClickSubscribeApi;

public class ProcessSubscribersFunction
{
    private readonly ISubscriptionProcessingService _subscriptionProcessingService;

    public ProcessSubscribersFunction(ISubscriptionProcessingService subscriptionProcessingService)
    {
        _subscriptionProcessingService = subscriptionProcessingService;
    }

    [Function(nameof(ProcessSubscribersFunction))]
    public Task RunAsync([TimerTrigger("0 */15 * * * *")]TimerInfo timerInfo)
        => _subscriptionProcessingService.ProcessSubscribersAsync();
}
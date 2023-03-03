namespace OneClickSubscribeApi.Domain.Services;

public interface ISubscriptionProcessingService
{
    Task<bool> TryProcessSubscriberAsync((string? email, string? firstName, string? lastName, string? type) values);
}
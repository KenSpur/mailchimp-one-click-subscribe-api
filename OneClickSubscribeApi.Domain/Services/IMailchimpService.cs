using OneClickSubscribeApi.Domain.Models;

namespace OneClickSubscribeApi.Domain.Services;

public interface IMailchimpService
{
    Task<IReadOnlyCollection<(Subscriber subscriber, bool succeeded, string details)>> TryAddSubscribersAsync(IReadOnlyCollection<Subscriber> subscribers);
}
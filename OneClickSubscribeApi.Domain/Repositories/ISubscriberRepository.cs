using OneClickSubscribeApi.Domain.Models;

namespace OneClickSubscribeApi.Domain.Repositories;

public interface ISubscriberRepository
{
    Task AddSubscriberAsync(Subscriber subscriber);
    Task<IReadOnlyCollection<Subscriber>> GetSubscribersAsync(State state);
    Task UpdateSubscribersStateAsync(IReadOnlyCollection<Subscriber> subscribers);
}
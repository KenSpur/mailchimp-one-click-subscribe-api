using OneClickSubscribeApi.Domain.Models;

namespace OneClickSubscribeApi.Domain.Repositories;

public interface ISubscriberRepository
{
    Task AddSubscriberAsync(Subscriber subscriber);
}
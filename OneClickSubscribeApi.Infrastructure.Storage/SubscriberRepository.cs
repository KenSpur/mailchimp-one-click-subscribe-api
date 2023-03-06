using Azure;
using Azure.Data.Tables;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Repositories;
using OneClickSubscribeApi.Infrastructure.Storage.Defaults;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;

namespace OneClickSubscribeApi.Infrastructure.Storage;

internal class SubscriberRepository : ISubscriberRepository
{
    private readonly TableClient _tableClient;

    public SubscriberRepository(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    private static string SubscriberPartitionKey => SubscriberPartitionKeyDefaults.ValidSubscriber;
    private static string InvalidSubscriberPartitionKey => SubscriberPartitionKeyDefaults.InvalidSubscriber;

    public async Task AddSubscriberAsync(Subscriber subscriber)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var partitionKey = subscriber.State == State.Invalid
            ? InvalidSubscriberPartitionKey
            : SubscriberPartitionKey;

        var entity = await _tableClient.GetEntityIfExistsAsync<SubscriberEntity>(partitionKey, subscriber.Email);

        if (entity.HasValue)
            return;

        await _tableClient.AddEntityAsync(new SubscriberEntity
        {
            PartitionKey = partitionKey,
            Email = subscriber.Email,
            Firstname = subscriber.Firstname,
            Lastname = subscriber.Lastname,
            State = subscriber.State,
            Type = subscriber.Type
        });
    }

    public async Task<IReadOnlyCollection<Subscriber>> GetSubscribersAsync(State state)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var subscribers = await _tableClient.QueryAsync<SubscriberEntity>(e => e.State == state).ToListAsync();

        return subscribers.Select(s => new Subscriber(
            s.Email,
            s.Firstname,
            s.Lastname,
            s.Type,
            s.State)).ToList();
    }

    public async Task UpdateSubscribersStateAsync(IReadOnlyCollection<Subscriber> subscribers)
    {
        foreach (var subscriber in subscribers)
            await _tableClient.UpdateEntityAsync(new SubscriberEntity
            {
                Email = subscriber.Email,
                State = subscriber.State
            }, ETag.All);
    }
}
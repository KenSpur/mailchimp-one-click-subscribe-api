using AutoMapper;
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
    private readonly IMapper _mapper;

    public SubscriberRepository(TableClient tableClient, IMapper mapper)
    {
        _tableClient = tableClient;
        _mapper = mapper;
    }

    private static string SubscriberPartitionKey => SubscriberPartitionKeyDefaults.ValidSubscriber;
    private static string InvalidSubscriberPartitionKey => SubscriberPartitionKeyDefaults.InvalidSubscriber;

    public async Task AddSubscriberAsync(Subscriber subscriber)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var partitionKey = subscriber.State == State.Invalid
            ? InvalidSubscriberPartitionKey
            : SubscriberPartitionKey;

        var existingEntity = await _tableClient.GetEntityIfExistsAsync<SubscriberEntity>(partitionKey, subscriber.Email);

        if (existingEntity.HasValue)
            return;

        var entity = _mapper.Map<SubscriberEntity>(subscriber);

        entity.PartitionKey = partitionKey;

        await _tableClient.AddEntityAsync(entity);
    }

    public async Task<IReadOnlyCollection<Subscriber>> GetSubscribersAsync(State state)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var entities = await _tableClient.QueryAsync<SubscriberEntity>(filter: 
            $"{nameof(SubscriberEntity.PartitionKey)} eq '{SubscriberPartitionKey}' and {nameof(SubscriberEntity.State)} eq '{state}'")
            .ToListAsync();

        var subscribers = _mapper.Map<IReadOnlyCollection<Subscriber>>(entities);

        return subscribers;
    }

    public async Task UpdateSubscribersStateAndDetailsAsync(IReadOnlyCollection<Subscriber> subscribers)
    {
        foreach (var subscriber in subscribers)
            await _tableClient.UpdateEntityAsync(new SubscriberEntity
            {
                PartitionKey = SubscriberPartitionKey,
                Email = subscriber.Email,
                State = subscriber.State,
                Details = subscriber.Details
            }, ETag.All);
    }
}
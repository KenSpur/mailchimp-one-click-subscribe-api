using Azure.Data.Tables;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Repositories;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;

namespace OneClickSubscribeApi.Infrastructure.Storage;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly TableClient _tableClient;

    public SubscriberRepository(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    private static string PartitionKey => "Subscribers";

    public async Task AddSubscriberAsync(Subscriber subscriber)
    {
        await _tableClient.CreateIfNotExistsAsync();
        var entity = await _tableClient.GetEntityIfExistsAsync<SubscriberEntity>(PartitionKey, subscriber.Email);

        if (entity.HasValue)
            return;

        await _tableClient.AddEntityAsync(new SubscriberEntity
        {
            PartitionKey = PartitionKey,
            Email = subscriber.Email,
            Firstname = subscriber.Firstname,
            Lastname = subscriber.Lastname,
            State = subscriber.State,
            Type = subscriber.Type
        });
    }
}
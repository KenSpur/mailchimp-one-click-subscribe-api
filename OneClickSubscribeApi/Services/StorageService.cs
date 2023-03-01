using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using OneClickSubscribeApi.Models;
using OneClickSubscribeApi.Options;

namespace OneClickSubscribeApi.Services;

public interface IStorageService
{
    Task AddSubscriberAsync(string email, string? type);
}

internal class StorageService : IStorageService
{
    private readonly TableClient _tableClient;
    private readonly ApplicationOptions _options;

    public StorageService(TableClient tableClient, IOptions<ApplicationOptions> options)
    {
        _tableClient = tableClient;
        _options = options.Value;
    }

    private static string PartitionKey => "Subscribers";

    public async Task AddSubscriberAsync(string email, string? type)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var subscriber = await _tableClient.GetEntityIfExistsAsync<Subscriber>(PartitionKey, email);

        if (subscriber.HasValue)
            return;

        var typeToAdd = _options.ValidTypes.FirstOrDefault(t => t.Equals(type, StringComparison.OrdinalIgnoreCase)) ??
                        _options.DefaultType;

        await _tableClient.AddEntityAsync(new Subscriber
        {
            PartitionKey = PartitionKey,
            Email = email,
            State = State.New,
            Type = typeToAdd
        });
    }
}
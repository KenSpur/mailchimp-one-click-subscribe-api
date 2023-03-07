using Azure;
using Azure.Data.Tables;
using OneClickSubscribeApi.Domain.Models;

namespace OneClickSubscribeApi.Infrastructure.Storage.Entities;

internal class SubscriberEntity : ITableEntity
{
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; }
    public string? Email
    {
        get=> RowKey;      
        set => RowKey = value;
    }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Type { get; set; }
    public State State { get; set; }
    public string? Details { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
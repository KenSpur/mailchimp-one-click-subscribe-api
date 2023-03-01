using Azure;
using Azure.Data.Tables;

namespace OneClickSubscribeApi.Models;

internal class Subscriber : ITableEntity
{
    public string? Email
    {
        get=> RowKey;      
        set => RowKey = value;
    }
    public string? PartitionKey { get; set; }
    public string? RowKey { get; set; }
    public string? Type { get; set; }
    public State State { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
namespace OneClickSubscribeApi.Infrastructure.Storage.Options;

public class StorageOptions
{
    public string TableStorageConnectionString { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
}
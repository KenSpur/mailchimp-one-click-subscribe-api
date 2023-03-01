namespace OneClickSubscribeApi.Options;

public class StorageOptions
{
    public static string Key => nameof(StorageOptions);

    public string TableStorageConnectionString { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
}
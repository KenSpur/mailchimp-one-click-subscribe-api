namespace OneClickSubscribeApi.Domain.Options;

public class SubscriptionOptions
{
    public string DefaultType { get; set; } = string.Empty;
    public ICollection<string> ValidTypes { get; set; } = new List<string>();
}
namespace OneClickSubscribeApi.Options;

public class ApplicationOptions
{
    public static string Key => nameof(ApplicationOptions);

    public string DefaultType { get; set; } = string.Empty;
    public ICollection<string> ValidTypes { get; set; } = new List<string>();
    public string RedirectToForm { get; set; } = string.Empty;
    public string RedirectToSubscribed { get; set; } = string.Empty;
}
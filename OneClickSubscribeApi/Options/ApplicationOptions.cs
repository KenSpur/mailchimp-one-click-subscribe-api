namespace OneClickSubscribeApi.Options;

public class ApplicationOptions
{
    public static string Key => nameof(ApplicationOptions);

    public string RedirectToForm { get; set; } = string.Empty;
    public string RedirectToSubscribed { get; set; } = string.Empty;
}
namespace OneClickSubscribeApi.Infrastructure.Http.Options;

public class HttpOptions
{
    public string MailchimpApiBaseUrl { get; set; } = string.Empty;
    public string MailchimpApiKey { get; set; } = string.Empty;
    public string MailchimpAudienceId { get; set; } = string.Empty;
    public string MailchimpTypeMergeTag { get; set; } = string.Empty;
}
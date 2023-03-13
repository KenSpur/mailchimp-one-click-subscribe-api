using System.Dynamic;
using System.Text;
using System.Text.Json;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Services;
using OneClickSubscribeApi.Infrastructure.Http.Options;

namespace OneClickSubscribeApi.Infrastructure.Http;

public class MailchimpService : IMailchimpService
{
    private readonly HttpClient _client;
    private readonly HttpOptions _options;

    public MailchimpService(HttpClient client, HttpOptions options)
    {
        _client = client;
        _options = options;
    }

    public async Task<IReadOnlyCollection<(Subscriber subscriber, bool succeeded, string details)>> TryAddSubscribersAsync(IReadOnlyCollection<Subscriber> subscribers)
    {
        var results = new List<(Subscriber subscriber, bool succeeded, string details)>();
        foreach (var subscriber in subscribers)
           results.Add(await TryAddSubscriberAsync(subscriber));

        return results;
    }

    private async Task<(Subscriber subscriber, bool succeeded, string details)> TryAddSubscriberAsync(Subscriber subscriber)
    {
        dynamic mergeFields = new ExpandoObject();
        if (!string.IsNullOrEmpty(_options.MailchimpTypeMergeTag) && subscriber.Type is not null)
            (mergeFields as IDictionary<string, object>)?.Add(_options.MailchimpTypeMergeTag, subscriber.Type);
        if(subscriber.Firstname is not null)
            mergeFields.FNAME = subscriber.Firstname;
        if(subscriber.Lastname is not null)
            mergeFields.LNAME = subscriber.Lastname;

        var member = new
        {
            email_address = subscriber.Email,
            status = "subscribed",
            merge_fields = mergeFields
        };

        var jsonContent = JsonSerializer.Serialize(member);

        var response = await _client.PostAsync($"lists/{_options.MailchimpAudienceId}/members", new StringContent(jsonContent, Encoding.UTF8, "application/json"));

        var details = await response.Content.ReadAsStringAsync();

        return (subscriber, response.IsSuccessStatusCode, details);
    }
}
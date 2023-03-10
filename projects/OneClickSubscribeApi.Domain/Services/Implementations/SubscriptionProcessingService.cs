using System.Text.RegularExpressions;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Options;
using OneClickSubscribeApi.Domain.Repositories;

namespace OneClickSubscribeApi.Domain.Services.Implementations;

internal class SubscriptionProcessingService : ISubscriptionProcessingService
{
    private readonly SubscriptionOptions _options;
    private readonly ISubscriberRepository _repository;
    private readonly IMailchimpService _mailchimpService;

    public SubscriptionProcessingService(SubscriptionOptions options, ISubscriberRepository repository, IMailchimpService mailchimpService)
    {
        _options = options;
        _repository = repository;
        _mailchimpService = mailchimpService;
    }

    public static string EmailValidationRegexPattern =>
        @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$";

    public async Task<bool> TryProcessSubscriberAsync((string? email, string? firstName, string? lastName, string? type) values)
    {
        var isValid = TryProcessValues(values, out var subscriber);

        await _repository.AddSubscriberAsync(subscriber);

        return isValid;
    }

    public async Task ProcessSubscribersAsync()
    {
        var subscribers = await _repository.GetSubscribersAsync(State.New);

        var listToProcess = new List<Subscriber>();
        foreach (var subscriber in subscribers)
        {
             listToProcess.Add(subscriber);
             if (listToProcess.Count != _options.MaxBatchSize) continue;

             // process the batch
            await ProcessSubscribersAsync(subscribers);
            listToProcess.Clear();
        }

        if(listToProcess.Any()) // process the rest
            await ProcessSubscribersAsync(subscribers);
    }

    private async Task ProcessSubscribersAsync(IReadOnlyCollection<Subscriber> subscribers)
    {
        var results = await _mailchimpService.TryAddSubscribersAsync(subscribers);

        foreach (var (subscriber, succeeded, details) in results)
            SetStateAndDetails(subscriber, succeeded, details);

        await _repository.UpdateSubscribersStateAndDetailsAsync(results.Select(r => r.subscriber).ToList());
    }

    private static void SetStateAndDetails(Subscriber subscriber, bool succeeded, string details)
    {
        subscriber.SetState(succeeded ? State.Added : State.FailedToAdd);
        subscriber.SetDetails(details);
    }

    private bool TryProcessValues((string? email, string? firstName, string? lastName, string? type) values, out Subscriber subscriber)
    {
        var (email, firstName, lastName, type) = values;

        var isEmailValid = !string.IsNullOrEmpty(email) && IsValidEmail(email);

        var typeToAdd = _options.ValidTypes.FirstOrDefault(t => t.Equals(type?.Trim(), StringComparison.OrdinalIgnoreCase)) ??
                        _options.DefaultType;

        var state = isEmailValid ? State.New : State.Invalid;

        subscriber = new Subscriber(firstName, lastName, email, typeToAdd, state);

        return isEmailValid;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var rx = new Regex(EmailValidationRegexPattern);
            return rx.IsMatch(email);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
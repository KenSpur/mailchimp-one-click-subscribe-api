using System.Text.RegularExpressions;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Options;
using OneClickSubscribeApi.Domain.Repositories;

namespace OneClickSubscribeApi.Domain.Services.Implementations;

internal class SubscriptionProcessingService : ISubscriptionProcessingService
{
    private readonly SubscriptionOptions _options;
    private readonly ISubscriberRepository _repository;

    public SubscriptionProcessingService(SubscriptionOptions options, ISubscriberRepository repository)
    {
        _options = options;
        _repository = repository;
    }

    public static string EmailValidationRegexPattern =>
        @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$";

    public async Task<bool> TryProcessSubscriberAsync((string? email, string? firstName, string? lastName, string? type) values)
    {
        var isValid = TryProcessValues(values, out var subscriber);

        await _repository.AddSubscriberAsync(subscriber);

        return isValid;
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
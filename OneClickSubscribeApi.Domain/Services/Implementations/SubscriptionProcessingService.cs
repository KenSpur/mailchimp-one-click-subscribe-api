using System.Net.Mail;
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

    public async Task<bool> TryProcessSubscriberAsync((string? email, string? firstName, string? lastName, string? type) values)
    {
        if (!TryProcessValues(values, out var subscriber))
            return false;

        await _repository.AddSubscriberAsync(subscriber);

        return true;
    }

    private bool TryProcessValues((string? email, string? firstName, string? lastName, string? type) values, out Subscriber subscriber)
    {
        subscriber = null!;

        var (email, firstName, lastName, type) = values;

        if (string.IsNullOrEmpty(email))
            return false;

        if (!IsValidEmail(email))
            return false;

        var typeToAdd = _options.ValidTypes.FirstOrDefault(t => t.Equals(type, StringComparison.OrdinalIgnoreCase)) ??
                        _options.DefaultType;

        subscriber = new Subscriber(firstName, lastName, email, typeToAdd, State.New);

        return true;
    }

    private static bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
            return false;

        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}
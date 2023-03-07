namespace OneClickSubscribeApi.Domain.Models;

public class Subscriber
{
    public Subscriber(string? firstname, string? lastname, string? email, string? type, State state)
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email?.ToLowerInvariant();
        Type = type;
        State = state;
    }

    public Subscriber(string? firstname, string? lastname, string? email, string? type, State state, string? details)
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email?.ToLowerInvariant();
        Type = type;
        State = state;
        Details = details;
    }

    public string? Firstname { get; }
    public string? Lastname { get; }
    public string? Email { get; }
    public string? Type { get; }
    public State State { get; private set; }
    public string? Details { get; private set; }

    internal void SetState(State state)
    {
        State = state;
    }

    internal void SetDetails(string details)
    {
        Details = details;
    }
}
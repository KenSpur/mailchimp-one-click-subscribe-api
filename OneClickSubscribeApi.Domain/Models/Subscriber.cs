namespace OneClickSubscribeApi.Domain.Models;

public class Subscriber
{
    public Subscriber(string? firstName, string? lastName, string? email, string? type, State state)
    {
        Firstname = firstName;
        Lastname = lastName;
        Email = email;
        Type = type;
        State = state;
    }

    public string? Firstname { get; }
    public string? Lastname { get; }
    public string? Email { get; }
    public string? Type { get; }
    public State State { get; }
}
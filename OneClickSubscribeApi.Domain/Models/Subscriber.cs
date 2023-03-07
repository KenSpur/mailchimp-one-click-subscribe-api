namespace OneClickSubscribeApi.Domain.Models;

public class Subscriber
{
    public Subscriber(string? firstname, string? lastname, string? email, string? type, State state)
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
        Type = type;
        State = state;
    }

    public string? Firstname { get; }
    public string? Lastname { get; }
    public string? Email { get; }
    public string? Type { get; }
    public State State { get; private set; }

    internal void SetState(State state)
    {
        State = state;
    }
}
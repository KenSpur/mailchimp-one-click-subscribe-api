using AutoMapper;
using FluentAssertions;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;
using OneClickSubscribeApi.Infrastructure.Storage.Mapper;

namespace OneClickSubscribeApi.Infrastructure.Storage.Tests;

public class SubscriberProfileShould
{
    [Fact]
    public void CorrectlyMapSubscriberOnSubscriberEntity()
    {
        // Arrange
        var subscriber = new Subscriber("fn", "ln", "e@m.ail", "t", State.Added, "empty");

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<SubscriberProfile>()).CreateMapper();

        // Act
        var entity = mapper.Map<SubscriberEntity>(subscriber);

        // Assert
        entity.Firstname.Should().Be(subscriber.Firstname);
        entity.Lastname.Should().Be(subscriber.Lastname);
        entity.Email?.ToLowerInvariant().Should().Be(subscriber.Email);
        entity.Type.Should().Be(subscriber.Type);
        entity.State.Should().Be(subscriber.State);
        entity.Details.Should().Be(subscriber.Details);
    }

    [Fact]
    public void CorrectlyMapSubscriberEntityOnSubscriber()
    {
        // Arrange
        var entity = new SubscriberEntity
        {
            Firstname = "nf",
            Lastname = "nl",
            Email = "e@mai.l",
            Type = "e",
            State = State.FailedToAdd,
            Details = "d"
        };

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<SubscriberProfile>()).CreateMapper();

        // Act
        var subscriber = mapper.Map<Subscriber>(entity);

        // Assert
        subscriber.Firstname.Should().Be(entity.Firstname);
        subscriber.Lastname.Should().Be(entity.Lastname);
        subscriber.Email.Should().Be(entity.Email?.ToLowerInvariant());
        subscriber.Type.Should().Be(entity.Type);
        subscriber.State.Should().Be(entity.State);
        subscriber.Details.Should().Be(entity.Details);
    }
}
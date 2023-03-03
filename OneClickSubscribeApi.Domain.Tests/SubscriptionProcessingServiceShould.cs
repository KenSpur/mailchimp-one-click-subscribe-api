using FluentAssertions;
using Moq;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Options;
using OneClickSubscribeApi.Domain.Repositories;
using OneClickSubscribeApi.Domain.Services.Implementations;

namespace OneClickSubscribeApi.Domain.Tests;

public class SubscriptionProcessingServiceShould
{
    public static IEnumerable<object[]> InvalidEmailData()
    {
        yield return new object[] { "john.doe@example..com", null!, null!, null! };
        yield return new object[] { "john..doe@example.com", null!, null!, null! };
        yield return new object[] { ".john.doe@example.com", null!, null!, null! };
        yield return new object[] { "john.doe@example+com", null!, null!, null! };
        yield return new object[] { "john.doe@example.", null!, null!, null! };
        yield return new object[] { "@example.com", null!, null!, null! };
        yield return new object[] { " ", null!, null!, null! };
        yield return new object[] { "", null!, null!, null! };
        yield return new object[] { null!, null!, null!, null! };
    }

    public static IEnumerable<object[]> ValidEmailData()
    {
        yield return new object[] { "john.doe+test@example.com", null!, null!, null! };
        yield return new object[] { "jane_doe1234@example.com", null!, null!, null! };
        yield return new object[] { "john.doe@example.co.uk", null!, null!, null! };
        yield return new object[] { "johndoe@example.travel", null!, null!, null! };
        yield return new object[] { "john.doe@example.us", null!, null!, null! };
        yield return new object[] { "john.doe@example.technology", null!, null!, null! };
        yield return new object[] { "john-doe@example.email", null!, null!, null! };
    }

    public static IEnumerable<object[]> BothValidAndInvalidEmailData =>
        InvalidEmailData().Concat(ValidEmailData());

    [Theory]
    [MemberData(nameof(InvalidEmailData))]
    public async Task ReturnFalseWhenInvalidEmailAddressesArePassed(string? email, string? firstname, string? lastname, string? type)
    {
        var values = (email, firstname, lastname, type);

        // Arrange
        var service = new SubscriptionProcessingService(new SubscriptionOptions(), new Mock<ISubscriberRepository>().Object);

        // Act
        var result = await service.TryProcessSubscriberAsync(values);

        // Assert   
        result.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ValidEmailData))]
    public async Task ReturnTrueWhenValidEmailAddressesArePassed(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);

        var service = new SubscriptionProcessingService(new SubscriptionOptions(), new Mock<ISubscriberRepository>().Object);

        // Act
        var result = await service.TryProcessSubscriberAsync(values);

        // Assert   
        result.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(BothValidAndInvalidEmailData))]
    public async Task PassASubscriberToTheRepoWenBothValidOrInvalidEmailsAreUsed(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);
        var repoMock = new Mock<ISubscriberRepository>();
        var service = new SubscriptionProcessingService(new SubscriptionOptions(), repoMock.Object);

        // Act
        await service.TryProcessSubscriberAsync(values);

        // Assert   
        repoMock.Verify(repo
            => repo.AddSubscriberAsync(It.IsAny<Subscriber>()));
    }

    [Theory]
    [MemberData(nameof(InvalidEmailData))]
    public async Task PassASubscriberToTheRepoWithAInvalidStateWhenInvalidEmailsAreUsed(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);
        var repoMock = new Mock<ISubscriberRepository>();
        var service = new SubscriptionProcessingService(new SubscriptionOptions(), repoMock.Object);

        // Act
        await service.TryProcessSubscriberAsync(values);

        // Assert   
        repoMock.Verify(repo
            => repo.AddSubscriberAsync(It.Is<Subscriber>(sub => sub.State == State.Invalid)));
    }

    [Theory]
    [MemberData(nameof(ValidEmailData))]
    public async Task PassASubscriberToTheRepoWithANewStateWhenValidEmailsAreUsed(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);
        var repoMock = new Mock<ISubscriberRepository>();
        var service = new SubscriptionProcessingService(new SubscriptionOptions(), repoMock.Object);

        // Act
        await service.TryProcessSubscriberAsync(values);

        // Assert   
        repoMock.Verify(repo
            => repo.AddSubscriberAsync(It.Is<Subscriber>(sub => sub.State == State.New)));
    }

    [Theory]
    [InlineData("john-doe@example.email", null, null, null)]
    [InlineData("john-doe@example.email", null, null, "text")]
    [InlineData("john-doe@example.email", null, null, " ")]
    [InlineData("john-doe@example.email", null, null, "")]
    public async Task UseTheDefaultTypeWhenANoneValidTypeIsSelected(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);

        var options = new SubscriptionOptions
        {
            ValidTypes = new List<string> { "valid type", "test" },
            DefaultType = "default"
        };
        var repoMock = new Mock<ISubscriberRepository>();
        var service = new SubscriptionProcessingService(options, repoMock.Object);

        // Act
        await service.TryProcessSubscriberAsync(values);

        // Assert   
        repoMock.Verify(repo
            => repo.AddSubscriberAsync(It.Is<Subscriber>(sub => sub.Type == options.DefaultType)));
    }

    [Theory]
    [InlineData("john-doe@example.email", null, null, "test")]
    [InlineData("john-doe@example.email", null, null, "Test")]
    [InlineData("john-doe@example.email", null, null, "A Type")]
    [InlineData("john-doe@example.email", null, null, "a type")]
    [InlineData("john-doe@example.email", null, null, "a type ")]
    [InlineData("john-doe@example.email", null, null, " a type")]
    public async Task UseTheSelectedTypeWhenAValidTypeIsSelected(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);

        var options = new SubscriptionOptions
        {
            ValidTypes = new List<string> { "Test", "A Type" },
            DefaultType = "default"
        };
        var repoMock = new Mock<ISubscriberRepository>();
        var service = new SubscriptionProcessingService(options, repoMock.Object);

        // Act
        await service.TryProcessSubscriberAsync(values);

        // Assert   
        repoMock.Verify(repo
            => repo.AddSubscriberAsync(It.Is<Subscriber>(sub => options.ValidTypes.Contains(sub.Type?? ""))));
    }

    [Theory]
    [InlineData("john-doe@example.email", null, "doe", "Test")]
    [InlineData("john-doe@example.email", "john", null, "test")]
    [InlineData("john-doe@example.email", "john", "doe", null)]
    [InlineData("john-doe@example.email", "john", "doe", "Test")]
    public async Task PassOnFirstAndLastnameValuesInAValidScenario(string? email, string? firstname, string? lastname, string? type)
    {
        // Arrange
        var values = (email, firstname, lastname, type);

        var options = new SubscriptionOptions
        {
            ValidTypes = new List<string> { "Test", "A Type" },
            DefaultType = "default"
        };
        var repoMock = new Mock<ISubscriberRepository>();
        var service = new SubscriptionProcessingService(options, repoMock.Object);

        // Act
        await service.TryProcessSubscriberAsync(values);

        // Assert   
        repoMock.Verify(repo
            => repo.AddSubscriberAsync(It.Is<Subscriber>(sub => sub.Firstname == firstname && sub.Lastname == lastname)));
    }
}
using AutoMapper;
using Azure;
using Azure.Data.Tables;
using Moq;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Infrastructure.Storage.Defaults;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;
using OneClickSubscribeApi.Infrastructure.Storage.Mapper;

namespace OneClickSubscribeApi.Infrastructure.Storage.Tests;

public class SubscriberRepositoryAddSubscriberAsyncShould
{
    [Fact]
    public async Task UseTheSubscriberPartitionKeyWhenTheSubscriberStateIsNew()
    {
        // Arrange
        var subscriber = new Subscriber(null, null, null, null, State.New);

        var tableClient = new Mock<TableClient>();
        var response = new Mock<NullableResponse<SubscriberEntity>>();
        response.Setup(r => r.HasValue).Returns(false);
        tableClient.Setup(t => t.GetEntityIfExistsAsync<SubscriberEntity>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            null,
            default
            )).Returns(Task.FromResult(response.Object));
        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<SubscriberEntity>(It.IsAny<object>())).Returns(new SubscriberEntity());

        var repo = new SubscriberRepository(tableClient.Object, mapper.Object);

        // Act
        await repo.AddSubscriberAsync(subscriber);

        // Assert
        tableClient.Verify(t 
            => t.AddEntityAsync(
                It.Is<SubscriberEntity>(e => e.PartitionKey == SubscriberPartitionKeyDefaults.ValidSubscriber), 
                It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UseTheInvalidSubscriberPartitionKeyWhenTheSubscriberStateIsInvalid()
    {
        // Arrange
        var subscriber = new Subscriber(null, null, null, null, State.Invalid);

        var tableClient = new Mock<TableClient>();
        var response = new Mock<NullableResponse<SubscriberEntity>>();
        response.Setup(r => r.HasValue).Returns(false);
        tableClient.Setup(t => t.GetEntityIfExistsAsync<SubscriberEntity>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            null,
            default
        )).Returns(Task.FromResult(response.Object));
        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<SubscriberEntity>(It.IsAny<object>())).Returns(new SubscriberEntity());

        var repo = new SubscriberRepository(tableClient.Object, mapper.Object);

        // Act
        await repo.AddSubscriberAsync(subscriber);

        // Assert
        tableClient.Verify(t
            => t.AddEntityAsync(
                It.Is<SubscriberEntity>(e => e.PartitionKey == SubscriberPartitionKeyDefaults.InvalidSubscriber),
                It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task OnlyAddEmailsAsLowerCaseStrings()
    {
        // Arrange
        var subscriber = new Subscriber(null, null, "UPPER.CASE@EMAIL.com", null, State.Invalid);

        var tableClient = new Mock<TableClient>();
        var response = new Mock<NullableResponse<SubscriberEntity>>();
        response.Setup(r => r.HasValue).Returns(false);
        tableClient.Setup(t => t.GetEntityIfExistsAsync<SubscriberEntity>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            null,
            default
        )).Returns(Task.FromResult(response.Object));
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<SubscriberProfile>()).CreateMapper();

        var repo = new SubscriberRepository(tableClient.Object, mapper);

        // Act
        await repo.AddSubscriberAsync(subscriber);

        // Assert
        tableClient.Verify(t
            => t.AddEntityAsync(
                It.Is<SubscriberEntity>(e => !e.Email!.Any(char.IsUpper)),
                It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task OnlyLookForEmailsAsLowerCaseStrings()
    {
        // Arrange
        var subscriber = new Subscriber(null, null, "UPPER.CASE@EMAIL.com", null, State.Invalid);

        var tableClient = new Mock<TableClient>();
        var response = new Mock<NullableResponse<SubscriberEntity>>();
        response.Setup(r => r.HasValue).Returns(false);
        tableClient.Setup(t => t.GetEntityIfExistsAsync<SubscriberEntity>(
            It.IsAny<string>(),
            It.IsAny<string>(),
            null,
            default
        )).Returns(Task.FromResult(response.Object));
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<SubscriberProfile>()).CreateMapper();

        var repo = new SubscriberRepository(tableClient.Object, mapper);

        // Act
        await repo.AddSubscriberAsync(subscriber);

        // Assert
        tableClient.Verify(t
            => t.GetEntityIfExistsAsync<SubscriberEntity>(It.IsAny<string>(),
                It.Is<string>(e => !e.Any(char.IsUpper)),
                null,
                default));
    }
}
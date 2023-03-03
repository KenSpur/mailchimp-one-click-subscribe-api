using Azure;
using Azure.Data.Tables;
using Moq;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Infrastructure.Storage.Defaults;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;

namespace OneClickSubscribeApi.Infrastructure.Storage.Tests
{
    public class SubscriberRepositoryShould
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

            var repo = new SubscriberRepository(tableClient.Object);

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

            var repo = new SubscriberRepository(tableClient.Object);

            // Act
            await repo.AddSubscriberAsync(subscriber);

            // Assert
            tableClient.Verify(t
                => t.AddEntityAsync(
                    It.Is<SubscriberEntity>(e => e.PartitionKey == SubscriberPartitionKeyDefaults.InvalidSubscriber),
                    It.IsAny<CancellationToken>()));
        }
    }
}
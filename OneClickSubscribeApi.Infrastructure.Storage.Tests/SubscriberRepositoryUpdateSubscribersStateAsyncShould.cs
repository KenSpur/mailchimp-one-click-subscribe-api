using AutoMapper;
using Azure;
using Azure.Data.Tables;
using Moq;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;

namespace OneClickSubscribeApi.Infrastructure.Storage.Tests;
public class SubscriberRepositoryUpdateSubscribersStateAsyncShould
{
    public static IEnumerable<Subscriber> Subscribers()
    {
        yield return new Subscriber("j", "d", "jd@example.com", "y", State.Added);
        yield return new Subscriber("d", "d", "dd@example.com", "x", State.FailedToAdd);
        yield return new Subscriber("a", "d", "ad@example.com", "a", State.Invalid);
        yield return new Subscriber("e", "d", "ed@example.com", "b", State.FailedToAdd);
        yield return new Subscriber("r", "d", "rd@example.com", "c", State.New);
    }

    [Fact]
    public async Task OnlyUpdateSubscriberState()
    {
        // Arrange
        var subscribers = Subscribers().ToList();

        var tableClient = new Mock<TableClient>();
        var mapper = new Mock<IMapper>();
        var repo = new SubscriberRepository(tableClient.Object, mapper.Object);

        var validation =
            (SubscriberEntity entity) =>
            {
                var subscriber = subscribers.Single(s => s.Email == entity.Email);

                var properties = typeof(SubscriberEntity).GetProperties();

                return properties.Where(p =>
                           p.Name != nameof(SubscriberEntity.State)
                           && p.Name != nameof(SubscriberEntity.Email)
                           && p.Name != nameof(SubscriberEntity.RowKey)
                           && p.Name != nameof(SubscriberEntity.ETag))
                       .All(p => p.GetValue(entity) is null)
                   && entity.State == subscriber.State;
            };
                
        // Act
        await repo.UpdateSubscribersStateAsync(subscribers);

        // Assert
        tableClient.Verify(t => t.UpdateEntityAsync(
            It.Is<SubscriberEntity>(e => validation.Invoke(e)),
            It.IsAny<ETag>(),
            It.IsAny<TableUpdateMode>(), 
            It.IsAny<CancellationToken>()));
    }
}
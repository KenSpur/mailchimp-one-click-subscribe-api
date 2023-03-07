using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Domain.Repositories;
using OneClickSubscribeApi.Domain.Services;

namespace OneClickSubscribeApi.Domain.Tests;

public class SubscriptionProcessingServiceProcessSubscribersAsyncShould : SubscriptionProcessingServiceTestBase
{
    public static IEnumerable<(Subscriber subscriber, bool success, string details)> SuccessfulMailchimpResults()
    {
        yield return ( new Subscriber(null, null, email: "test@success.com", null, State.New), true, string.Empty);
        yield return ( new Subscriber(null, null, email: "test@successful.net", null, State.New), true, string.Empty);
        yield return ( new Subscriber(null, null, email: "success@test.net", null, State.New), true, string.Empty);
        yield return ( new Subscriber(null, null, email: "a@examples.c", null, State.New), true, string.Empty);
    }

    public static IEnumerable<(Subscriber subscriber, bool success, string details)> FailedMailchimpResults()
    {
        yield return ( new Subscriber(null, null, email: "test@failed.com", null, State.New), false, string.Empty) ;
        yield return ( new Subscriber(null, null, email: "test@failiure.net", null, State.New), false, string.Empty);
        yield return ( new Subscriber(null, null, email: "failed@test.net", null, State.New), false, string.Empty);
        yield return ( new Subscriber(null, null, email: "t@example.c", null, State.New), false, string.Empty);
    }

    public static IReadOnlyCollection<(Subscriber subscriber, bool success, string details)> BothSuccessfulAndFailedMailchimpResults =>
        SuccessfulMailchimpResults().Concat(FailedMailchimpResults()).ToList();

    [Fact]
    public async Task SetStateToAddedWhenMailchimpServiceReturnsSuccess()
    {
        // Arrange
        var repository = new Mock<ISubscriberRepository>();
        var mailchimpService = new Mock<IMailchimpService>();
        mailchimpService.Setup(m => m.TryAddSubscribersAsync(It.IsAny<IReadOnlyCollection<Subscriber>>()))
            .ReturnsAsync(BothSuccessfulAndFailedMailchimpResults);

        var service = CreateSubscriptionProcessingService(repository: repository.Object, mailchimpService: mailchimpService.Object);

        // Act
        await service.ProcessSubscribersAsync();

        // Assert
        repository.Verify(r =>
            r.UpdateSubscribersStateAndDetailsAsync(It.Is<IReadOnlyCollection<Subscriber>>(
                subs => subs.Where(s =>
                        SuccessfulMailchimpResults().Any(result => result.subscriber.Email == s.Email))
                    .All(s => s.State == State.Added))));
    }

    [Fact]
    public async Task SetStateToFailedToAddWhenMailchimpServiceReturnsNotSuccess()
    {
        // Arrange
        var repository = new Mock<ISubscriberRepository>();
        var mailchimpService = new Mock<IMailchimpService>();
        mailchimpService.Setup(m => m.TryAddSubscribersAsync(It.IsAny<IReadOnlyCollection<Subscriber>>()))
            .ReturnsAsync(BothSuccessfulAndFailedMailchimpResults);

        var service = CreateSubscriptionProcessingService(repository: repository.Object, mailchimpService: mailchimpService.Object);

        // Act
        await service.ProcessSubscribersAsync();

        // Assert
        repository.Verify(r => 
            r.UpdateSubscribersStateAndDetailsAsync(It.Is<IReadOnlyCollection<Subscriber>>(
                subs => subs.Where(s => 
                        FailedMailchimpResults().Any(result => result.subscriber.Email == s.Email))
                    .All(s => s.State == State.FailedToAdd))));
    }
}
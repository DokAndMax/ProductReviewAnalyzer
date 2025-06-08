using MassTransit;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Sagas;

public class ProductReviewSaga : SagaStateMachineInstance
{
    private volatile int expectedReviewsCount;
    private volatile int receivedReviewsCount;

    public Guid CorrelationId { get; set; }
    public State CurrentState { get; set; } = default!;
    public int ExpectedProductsCount { get; set; }
    public int ExpectedReviewsCount { get => expectedReviewsCount;set => expectedReviewsCount = value; }
    public int ReceivedReviewsCount { get => receivedReviewsCount; set => receivedReviewsCount = value; }
    public ICollection<long> ProductsId { get; set; } = [];

    public void IncrementReceivedReviewsCount()
    {
        Interlocked.Increment(ref receivedReviewsCount);
    }

    public void AddExpectedReviewsCount(int additionalReviewsCount)
    {
        Interlocked.Add(ref expectedReviewsCount, additionalReviewsCount);
    }
}
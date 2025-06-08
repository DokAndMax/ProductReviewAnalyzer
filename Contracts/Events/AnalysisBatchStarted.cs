namespace ProductReviewAnalyzer.Contracts.Events;

public record AnalysisBatchStarted(
    Guid RequestId,
    int TotalProducts);
